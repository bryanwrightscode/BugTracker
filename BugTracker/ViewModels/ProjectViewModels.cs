﻿using BugTracker.Models.CodeFirst;
using BugTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class ProjectViewModel : ApplicationBaseViewModel
    {
        public ApplicationUser User { get; set; }
        public Project Assigned { get; set; }
        public Project All { get; set; }
    }

    public class AllAndAssignedProjectsViewModel : ApplicationBaseViewModel
    {
        public ICollection<Project> Assigned { get; set; }
        public ICollection<Project> All { get; set; }
    }

    public class ProjectAssignViewModel : ApplicationBaseViewModel
    {
        public Project AssignProject { get; set; }
        public MultiSelectList Users { get; set; }
        public string[] SelectedUsers { get; set; }
    }

    public class CreateProjectViewModel : ApplicationBaseViewModel
    {
        [Required]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long", MinimumLength = 6)]
        public string Title { get; set; }
        [Required]
        [StringLength(240, ErrorMessage = "The {0} must be at least {2} characters long", MinimumLength = 6)]
        public string Description  { get; set; }
    }
}