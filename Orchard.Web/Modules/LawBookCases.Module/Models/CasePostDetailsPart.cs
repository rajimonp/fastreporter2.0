using LawBookCases.Module.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LawBookCases.Module.Models
{
    [OrchardFeature("LawBookCases.Module.Attributes")]
    public class CasePostAttribRecord: ContentPartRecord
    {
        public virtual int CaseNumber { get; set; }
        public virtual int CaseYear {get;set;}
        //It is the reference to the key of the actual post
        public virtual int CasePostNumber { get; set; }
        public virtual  string CaseDecision { get; set; }
        public virtual string CaseClient1 { get; set; }
        public virtual string CaseClient2 { get; set; }
        public virtual string CaseHeldIn { get; set; }//Court type
        public virtual int CaseAcquiredBy { get; set; }
        public virtual string CaseAcquiredRole { get; set; }
        public virtual string CaseDecisionTakenBy { get; set; }
        public virtual string CaseHeldCourt { get; set; }//Actual Court Name 
        public virtual string CaseReference { get; set; }//Actual Court Name 


    }

    [OrchardFeature("LawBookCases.Module.Attributes")]
    public class CasePostAttribPart:ContentPart<CasePostAttribRecord>
    {
        
        [DisplayName("Post id tomake relati")]
        public int CaseNumber
        {
            get { return Retrieve(r => r.CaseNumber); }
            set { Store(r => r.CaseNumber, value); }
        }

        [DisplayName("Case Number")]
        public int CasePostNumber
        {
            get { return Retrieve(r => r.CasePostNumber); }
            set { Store(r => r.CasePostNumber, value); }
        }

        [DisplayName("Case Held on (Year)")]
        [Required]
        public int CaseYear
        {
            get { return Retrieve(r => r.CaseYear); }
            set { Store(r => r.CaseYear, value); }
        }

        [DisplayName("Case Decision ")]
        [Required]
        public string CaseDecision
        {
            get { return Retrieve(r => r.CaseDecision); }
            set { Store(r => r.CaseDecision, value); }
        }

        [DisplayName("Case Decision Taken  By")]
        
        public string CaseDecisionTakenBy
        {
            get { return Retrieve(r => r.CaseDecisionTakenBy); }
            set { Store(r => r.CaseDecisionTakenBy, value); }
        }

        [DisplayName("1st Party")]
        
        public string CaseClient1
        {
            get
            {
                return Retrieve(r => r.CaseClient1);
            }
            set
            {
                Store(r => r.CaseClient1, value);
            }
        }
        [DisplayName("2nd Party")]
      
        public string CaseClient2
        {
            get
            {
                return Retrieve(r => r.CaseClient2);
            }
            set
            {
                Store(r => r.CaseClient2, value);
            }
        }
        [DisplayName("Case Held at (Court Name")]
      

        public string CaseHeldIn
        {
            get
            {
                return Retrieve(r => r.CaseHeldIn);
            }
            set
            {
                Store(r => r.CaseHeldIn, value);
            }
        }

        [DisplayName("Court Name")]
    

        public string CaseHeldCourt
        {
            get
            {
                return Retrieve(r => r.CaseHeldCourt);
            }
            set
            {
                Store(r => r.CaseHeldCourt, value);
            }
        }

        [DisplayName("Case Reference")]
   

        public string CaseReference
        {
            get
            {
                return Retrieve(r => r.CaseReference);
            }
            set
            {
                Store(r => r.CaseReference, value);
            }
        }

        [DisplayName("Current Owner")]
        public int CaseAcquiredBy
        {
            get
            {
                return Retrieve(r => r.CaseAcquiredBy);
            }
            set
            {
                Store(r => r.CaseAcquiredBy, value);
            }
        }

        [DisplayName("Current Owner")]
        public string CaseAcquiredRole
        {
            get
            {
                return Retrieve(r => r.CaseAcquiredRole);
            }
            set
            {
                Store(r => r.CaseAcquiredRole, value);
            }

        }
        public int currentCaseIndx { get; set; }
        public string Genre { get; set; }

        public IEnumerable<SelectListItem> GenreList
        {
            get
            {
                yield return new SelectListItem { Text = "HC", Value = "1" };
                yield return new SelectListItem { Text = "PO", Value = "2" };
                yield return new SelectListItem { Text = "IPAB", Value = "3" };
                yield return new SelectListItem { Text = "SC", Value = "4" };
            }
        }

        public CasePostStateRecord CurrentPostState{get;set;}

        public IEnumerable<SelectListItem> CourtList   { get; set; }
    }


}