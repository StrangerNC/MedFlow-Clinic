using AutoMapper;
using Grpc.Core;
using PatientService.Data;
using PatientService.Dtos;

namespace PatientService.SyncDataService;

public class GrpcPatient(IRepository repository, IMapper mapper) : PatientService.GrpcPatient.GrpcPatientBase
{
    private readonly IRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public override async Task GetAllPatients(GrpcEmptyRequest request,
        IServerStreamWriter<GrpcPatientResponse> responseStream, ServerCallContext context)
    {
        var patients = await _repository.GetPatients();
        foreach (var patient in patients)
        {
            var dto = _mapper.Map<PatientPublishDto>(patient);
            await responseStream.WriteAsync(new GrpcPatientResponse()
            {
                PatientId = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                MiddleName = patient.MiddleName,
                PhoneNumber = patient.PhoneNumber
            });
        }

        Console.WriteLine("-->[INFO] Grpc data was send");
        await Task.CompletedTask;
    }

    public override async Task GetAllPatientsForTransfer(IAsyncStreamReader<GrpcPatientRequest> requestStream,
        IServerStreamWriter<GrpcPatientResponse> responseStream,
        ServerCallContext context)
    {
        var patients = await _repository.GetPatients();
        var updateTasks = new List<Task>();
        var updatePatients = new List<GrpcPatientRequest>();

        foreach (var patient in patients)
        {
            var dto = _mapper.Map<PatientPublishDto>(patient);
            await responseStream.WriteAsync(new GrpcPatientResponse()
            {
                PatientId = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                MiddleName = patient.MiddleName,
                PhoneNumber = patient.PhoneNumber
            });
        }

        await responseStream.WriteAsync(new GrpcPatientResponse()
        {
            PatientId = -1,
            FirstName = "",
            LastName = "",
            MiddleName = "",
            PhoneNumber = ""
        });

        await foreach (var msg in requestStream.ReadAllAsync())
        {
            updateTasks.Add(Task.Run(() => { updatePatients.Add(msg); }));
        }

        await Task.WhenAll(updateTasks);
        foreach (var patient in updatePatients)
        {
            _repository.UpdateTrasferredPatientStatus(patient.PatientId, patient.IsTransferred);
        }

        _repository.SaveChanges();
        Console.WriteLine("-->[INFO] Grpc data was send");

        await Task.CompletedTask;
    }
}