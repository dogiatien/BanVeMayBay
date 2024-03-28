using BanVeMayBay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanVeMayBay.DesignPattern.Decorator
{
    public class NoteDecorator : Iticket
    {
        private readonly Iticket _ticket;
        private readonly string _note;

        public NoteDecorator(Iticket ticket, string note)
        {
            _ticket = ticket;
            _note = note;
        }

        public string Description => _ticket.Description + ", " + _note;
    }
}