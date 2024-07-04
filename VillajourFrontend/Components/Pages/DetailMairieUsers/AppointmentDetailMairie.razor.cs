using Microsoft.AspNetCore.Components;
using Radzen;
using System.Globalization;
using System.Security.Claims;
using VillajourFrontend.Entity;

namespace VillajourFrontend.Components.Pages.DetailMairieUsers;

public partial class AppointmentDetailMairie
{
    [Parameter]
    public Guid idMairie { get; set; }

    [Inject]
    protected DialogService DialogService { get; set; }

    [Inject]
    protected HttpClient HttpClient { get; set; }
    
    [Inject]
    private IHttpContextAccessor? _httpContext { get; set; }
    protected Guid UserGuid { get => new Guid(_httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value); }

    private DateTime selectedDate = DateTime.Today;
    private List<string> availableSlots = new List<string>();
    private List<Appointment> existingAppointments = new List<Appointment>();

    private int selectedDay;
    private int selectedMonth;
    private int selectedYear;

    private List<int> days = Enumerable.Range(1, 31).ToList();
    private List<int> years = Enumerable.Range(2020, 10).ToList();
    private List<Month> months = DateTimeFormatInfo.CurrentInfo.MonthNames
        .Where(m => !string.IsNullOrEmpty(m))
        .Select((m, index) => new Month { Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(m), Value = index + 1 })
        .ToList();

    protected override async Task OnInitializedAsync()
    {
        selectedDay = selectedDate.Day;
        selectedMonth = selectedDate.Month;
        selectedYear = selectedDate.Year;

        await LoadAppointments();
        LoadSlots();
    }

    private async Task LoadAppointments()
    {
        var apiUrl = $"Appointments/GetAppointmentByMairie/{idMairie}";
        try
        {
            existingAppointments = await HttpClient.GetFromJsonAsync<List<Appointment>>(apiUrl) ?? new List<Appointment>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement des rendez-vous: {ex.Message}");
        }
    }

    private void LoadSlots()
    {
        availableSlots.Clear();
        var isPastDate = selectedDate < DateTime.Today;
        for (var time = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 8, 0, 0); time < new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, 18, 0, 0); time = time.AddMinutes(30))
        {
            if (isPastDate || IsSlotTaken(time))
            {
                availableSlots.Add($"{time:HH:mm} (Réservé)");
            }
            else
            {
                availableSlots.Add(time.ToString("HH:mm"));
            }
        }
    }

    private async Task OnDateChange()
    {
        try
        {
            selectedDate = new DateTime(selectedYear, selectedMonth, selectedDay);
            await LoadAppointments();
            LoadSlots();
        }
        catch
        {
            // Handle invalid date combinations (e.g., February 30)
        }
    }

    private bool IsSlotTaken(DateTime slotStartTime)
    {
        return existingAppointments.Any(a => a.StartTime == slotStartTime);
    }

    private async Task OpenAppointmentForm(string startTime)
    {
        if (startTime.Contains("(Réservé)"))
        {
            return; // Prevent opening form for taken slots
        }

        var start = DateTime.ParseExact(startTime.Split(' ')[0], "HH:mm", CultureInfo.InvariantCulture);
        var appointment = new Appointment
        {
            StartTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, start.Hour, start.Minute, 0),
            EndTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, start.Hour, start.Minute, 0).AddMinutes(30),
            UserId = UserGuid,
            MairieId = idMairie
        };

        var parameters = new Dictionary<string, object> { { "CurrentAppointment", appointment } };
        var options = new DialogOptions() { Width = "700px", Height = "450px" };
        var result = await DialogService.OpenAsync<AppointmentForm>("Nouveau Rendez-vous", parameters, options);

        if (result != null)
        {
            existingAppointments.Add(result);
            LoadSlots();
        }
    }
}

public class Month
{
    public string Text { get; set; }
    public int Value { get; set; }
}
