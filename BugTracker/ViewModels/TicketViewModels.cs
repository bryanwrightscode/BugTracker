using BugTracker.Models.CodeFirst;
using BugTracker.Models.Helpers;
using BugTracker.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class TicketsIndexViewModel : ApplicationBaseViewModel
    {
        public ICollection<Ticket> Tickets { get; set; }
    }

    public class NewTicketViewModel : ApplicationBaseViewModel
    {
        [Required(ErrorMessage = "A title is required")]
        [Display(Name = "Ticket Title")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long", MinimumLength = 6)]
        public string Title { get; set; }

        [Required(ErrorMessage = "A description is required")]
        [Display(Name = "Describe the issue")]
        [StringLength(280, ErrorMessage = "The {0} must be at least {2} characters long", MinimumLength = 6)]
        public string Description { get; set; }

        [Required(ErrorMessage = "A project is required")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "A ticket type is required")]
        public int TicketTypeId { get; set; }

        [Required(ErrorMessage = "A ticket priority is required")]
        public int TicketPriorityId { get; set; }

        public SelectList ProjectList { get; set; }
        public SelectList TypeList { get; set; }
        public SelectList PriorityList { get; set; }
    }

    //public class EditTicketViewModel
    //{
    //    public Ticket Ticket { get; set; }
    //    public SelectList TicketTypeId { get; set; }
    //    public SelectList TicketPriorityId { get; set; }
    //    public SelectList TicketStatusId { get; set; }
    //    public SelectList AssignToUserId { get; set; }
    //}

    public class EditTicketViewModel : ApplicationBaseViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "A title is required")]
        [Display(Name = "Ticket Title")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long", MinimumLength = 6)]
        public string Title { get; set; }
        [Required(ErrorMessage = "A description is required")]
        [Display(Name = "Describe the issue")]
        [StringLength(280, ErrorMessage = "The {0} must be at least {2} characters long", MinimumLength = 6)]
        public string Description { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        public string AssignToUserId { get; set; }

        public SelectList TypeList { get; set; }
        public SelectList PriorityList { get; set; }
        public SelectList StatusList { get; set; }
        public SelectList AssignToUserList { get; set; }
    }


    public class TicketDetailViewModel : ApplicationBaseViewModel
    {
        public Ticket Ticket {  get; set; }
        public ICollection<TicketComment> TicketComments { get; set; }
        public ICollection<TicketAttachment> TicketAttachments { get; set; }
        public ICollection<TicketHistory> TicketHistories { get; set; }
        public ApplicationUser User { get; set; }
        public TicketComment TicketComment { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string Description { get; set; }
    }

    public class TicketCommentEditModel : ApplicationBaseViewModel
    {
        public Ticket Ticket { get; set; }
        public TicketComment TicketComment { get; set; }
    }

    public class TicketAttachmentEditModel : ApplicationBaseViewModel
    {
        public int Id { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string Description { get; set; }
    }

}