namespace VillajourFrontend.Entity
{
    public class Appointment
    {
        public string Id { get; set; }
        public int UserID { get; set; }
        public int MairieId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Validation { get; set; }
        public int AppointmentTypeId { get; set; }
    }
}
