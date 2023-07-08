﻿namespace Talkie.DTOs.Account
{
    public class GetAccountDto
    {
        public string Number { get; set; } = string.Empty;

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
}