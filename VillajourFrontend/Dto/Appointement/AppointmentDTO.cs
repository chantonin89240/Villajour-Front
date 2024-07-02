namespace VillajourFrontend.Dto.Appointement
{
    public class AppointmentDTO
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Statut { get; set; }
        public int AppointmentTypeId { get; set; }
        public string AppointmentTypeLibelle { get; set; }
        public Guid MairieId { get; set; }
        public Guid UserId { get; set; }
    }
}
