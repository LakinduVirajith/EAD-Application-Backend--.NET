﻿namespace EAD_Backend_Application__.NET.Models
{
    public class UpdateEmailModel
    {
        public string CurrentEmail { get; set; } = string.Empty;
        public string NewEmail { get; set; } = string.Empty;

        public UpdateEmailModel()
        {
            
        }
    }
}