﻿using System;
using System.Windows.Media;

namespace Client.Controls.WpfScheduler
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime Start {get; set; }
        public DateTime End { get; set; }
        public bool AllDay { get; set; }
        public Brush Color { get; set; }

        public Event()
        {
            Id = Guid.NewGuid();
        }
    }
}