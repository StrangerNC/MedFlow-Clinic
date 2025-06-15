using AutoMapper;
using Grpc.Net.Client;
using TransferService.Data;
using TransferService.Models;

namespace TransferService.SyncDataService;

public class SendDataClient(IRepository repository, IMapper mapper) : ISendDataClient
{
    private readonly IRepository _repository = repository;
    private readonly IMapper _mapper = mapper;
    private string minzdravAddress = "";

    public async Task SendData()
    {
        try
        {
            var appointments = await _repository.GetAppointmentsToSend();
            var medicalRecords = await _repository.GetMedicalRecordsToSend();
            var visits = await _repository.GetVisitsToSend();
            var userProfiles = await _repository.GetUserProfilesToSend();
            var patients = await _repository.GetPatientsToSend();
            var clinicData = await _repository.GetClinicData();
            minzdravAddress = clinicData.MinzdravIPAddress;

            await SendClinicData(_mapper.Map<TransferClinicDataRequest>(clinicData));
            await SendUserProfiles(_mapper.Map<IEnumerable<TransferUserProfileRequest>>(userProfiles));
            await SendPatients(_mapper.Map<IEnumerable<TransferPatientRequest>>(patients));
            await SendAppointments(_mapper.Map<IEnumerable<TransferAppointmentRequest>>(appointments));
            await SendMedicalRecords(_mapper.Map<IEnumerable<TransferMedicalRecordRequest>>(medicalRecords));
            await SendVisits(_mapper.Map<IEnumerable<TransferVisitRequest>>(visits));
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Failed to send data visit records {e}");
        }
    }

    private async Task SendClinicData(TransferClinicDataRequest request)
    {
        try
        {
            Console.WriteLine("-->[INFO] Sending Clinic Data...");
            var channel = GrpcChannel.ForAddress(minzdravAddress);
            var client = new GrpcTransferData.GrpcTransferDataClient(channel);
            var call = client.TransferClinicData(request);
            Console.WriteLine("-->[INFO] Clinic Data sent");
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Sending clinic data failed {e}");
        }
    }

    private async Task SendAppointments(IEnumerable<TransferAppointmentRequest> appointments)
    {
        try
        {
            Console.WriteLine("-->[INFO] Sending appointments...");
            var channel = GrpcChannel.ForAddress(minzdravAddress);
            var client = new GrpcTransferData.GrpcTransferDataClient(channel);
            var call = client.TransferAppointment();
            foreach (var appointment in appointments)
            {
                await call.RequestStream.WriteAsync(appointment);
            }

            await call.RequestStream.CompleteAsync();
            Console.WriteLine("-->[INFO] appointments sent");
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Sending appointments failed {e}");
        }
    }

    private async Task SendMedicalRecords(IEnumerable<TransferMedicalRecordRequest> medicalRecords)
    {
        try
        {
            Console.WriteLine("-->[INFO] Sending medical-records...");
            var channel = GrpcChannel.ForAddress(minzdravAddress);
            var client = new GrpcTransferData.GrpcTransferDataClient(channel);
            var call = client.TransferMedicalReport();
            foreach (var record in medicalRecords)
            {
                await call.RequestStream.WriteAsync(record);
            }

            await call.RequestStream.CompleteAsync();
            Console.WriteLine("-->[INFO] medical-records sent");
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Sending medical records failed {e}");
        }
    }

    private async Task SendVisits(IEnumerable<TransferVisitRequest> visits)
    {
        try
        {
            Console.WriteLine("-->[INFO] Sending visits...");
            var channel = GrpcChannel.ForAddress(minzdravAddress);
            var client = new GrpcTransferData.GrpcTransferDataClient(channel);
            var call = client.TransferVisit();
            foreach (var visit in visits)
            {
                await call.RequestStream.WriteAsync(visit);
            }

            await call.RequestStream.CompleteAsync();
            Console.WriteLine("-->[INFO] visits sent");
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Sending visits failed {e}");
        }
    }

    private async Task SendUserProfiles(IEnumerable<TransferUserProfileRequest> userProfiles)
    {
        try
        {
            Console.WriteLine("-->[INFO] Sending user-profiles...");
            var channel = GrpcChannel.ForAddress(minzdravAddress);
            var client = new GrpcTransferData.GrpcTransferDataClient(channel);
            var call = client.TransferUserProfile();
            foreach (var profile in userProfiles)
            {
                await call.RequestStream.WriteAsync(profile);
            }

            await call.RequestStream.CompleteAsync();
            Console.WriteLine("-->[INFO] user-profiles sent");
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Sending user-profiles failed {e}");
        }
    }

    private async Task SendPatients(IEnumerable<TransferPatientRequest> patients)
    {
        try
        {
            Console.WriteLine("-->[INFO] Sending patients...");
            var channel = GrpcChannel.ForAddress(minzdravAddress);
            var client = new GrpcTransferData.GrpcTransferDataClient(channel);
            var call = client.TransferPatient();
            foreach (var patient in patients)
            {
                await call.RequestStream.WriteAsync(patient);
            }

            await call.RequestStream.CompleteAsync();
            Console.WriteLine("-->[INFO] patients sent");
        }
        catch (Exception e)
        {
            Console.WriteLine($"-->[ERROR] Sending patients failed {e}");
        }
    }
}