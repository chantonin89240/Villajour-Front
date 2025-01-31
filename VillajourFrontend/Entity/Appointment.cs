﻿namespace VillajourFrontend.Entity
{
    public class Appointment
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MairieId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Statut { get; set; }
        public int AppointmentTypeId { get; set; }
    }
}
