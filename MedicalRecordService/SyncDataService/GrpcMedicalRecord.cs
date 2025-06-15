using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MedicalRecordService.Data;
using MedicalRecordService.Dtos;
using MedicalRecordService.Models;
using MedicalRecordService.Utils;

namespace MedicalRecordService.SyncDataService;

public class GrpcMedicalRecord(IRepository repository, IMapper mapper)
    : MedicalRecordService.GrpcMedicalRecord.GrpcMedicalRecordBase
{
    private readonly IRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public override async Task GetMedicalRecordForTransfer(IAsyncStreamReader<GrpcMedicalRecordRequest> requestStream,
        IServerStreamWriter<GrpcMedicalRecordResponse> responseStream,
        ServerCallContext context)
    {
        var medicalRecords = await _repository.GetMedicalRecords();
        var updateTask = new List<Task>();
        var updateMedicalRecords = new List<GrpcMedicalRecordRequest>();
        foreach (var medicalRecord in medicalRecords)
        {
            var dto = _mapper.Map<MedicalRecordPublishDto>(medicalRecord);
            await responseStream.WriteAsync(new GrpcMedicalRecordResponse()
            {
                MedicalRecordId = medicalRecord.Id,
                PatientId = medicalRecord.PatientId
            });
        }

        await responseStream.WriteAsync(new GrpcMedicalRecordResponse()
        {
            MedicalRecordId = -1,
            PatientId = -1,
        });

        await foreach (var msg in requestStream.ReadAllAsync())
        {
            updateTask.Add(Task.Run(() => updateMedicalRecords.Add(msg)));
        }

        await Task.WhenAll(updateTask);
        foreach (var medicalRecord in updateMedicalRecords)
        {
            _repository.UpdateMedicalRecordTransferStatus(medicalRecord.MedicalRecordId, medicalRecord.IsTransferred);
        }

        _repository.SaveChanges();
        Console.WriteLine("-->[INFO] Grpc data was send");

        await Task.CompletedTask;
    }

    public override async Task GetVisitForTransfer(IAsyncStreamReader<GrpcVisitRequest> requestStream,
        IServerStreamWriter<GrpcVisitResponse> responseStream,
        ServerCallContext context)
    {
        var visits = await _repository.GetVisits();
        var updateTask = new List<Task>();
        var updateVisits = new List<GrpcVisitRequest>();
        foreach (var visit in visits)
        {
            var dto = _mapper.Map<VisitPublishDto>(visit);
            await responseStream.WriteAsync(new GrpcVisitResponse
            {
                VisitId = visit.Id,
                MedicalRecordId = visit.MedicalRecordId,
                AppointmentId = visit.AppointmentId,
                VisitDate = visit.VisitDate.ToUtcKind().ToTimestamp(),
                ChiefComplaint = visit.ChiefComplaint,
                Diagnosis = visit.Diagnosis,
                TreatmentPlan = visit.TreatmentPlan,
                Notes = visit.Notes,
            });
        }

        await responseStream.WriteAsync(new GrpcVisitResponse
        {
            VisitId = -1,
            MedicalRecordId = -1,
            AppointmentId = -1,
            VisitDate = new Google.Protobuf.WellKnownTypes.Timestamp(),
            ChiefComplaint = "",
            Diagnosis = "",
            TreatmentPlan = "",
            Notes = ""
        });
        await foreach (var msg in requestStream.ReadAllAsync())
        {
            updateTask.Add(Task.Run(() => updateVisits.Add(msg)));
        }

        await Task.WhenAll(updateTask);
        foreach (var visit in updateVisits)
        {
            _repository.UpdateVisitTransferStatus(visit.VisitId, visit.IsTransferred);
        }

        _repository.SaveChanges();
        Console.WriteLine("-->[INFO] Grpc data was send");
        await Task.CompletedTask;
    }
}