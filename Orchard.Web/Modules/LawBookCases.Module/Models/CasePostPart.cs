using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Common.Models;
using Orchard.Security;
using Orchard.Core.Title.Models;
using Orchard.ContentManagement.Records;
using System.ComponentModel;
using System.Collections.Generic;
using LawBookCases.Module.Services;

namespace LawBookCases.Module.Models
{

    public class CasePostsPartRecord : ContentPartRecord
    {
        public virtual int CaseNumber { get; set; }

    }
    public class CasePostPart : ContentPart<CasePart>
    {
       

        public string Title
        {
            get { return this.As<TitlePart>().Title; }
            set { this.As<TitlePart>().Title = value; }
        }

        public string Text
        {
            get { return this.As<BodyPart>().Text; }
            set { this.As<BodyPart>().Text = value; }
        }

        public CasePart CasePart
        {
            get { return this.As<ICommonPart>().Container.As<CasePart>(); }
            set { this.As<ICommonPart>().Container = value; }
        }

        public IUser Creator
        {
            get { return this.As<ICommonPart>().Owner; }
            set { this.As<ICommonPart>().Owner = value; }
        }

        public bool IsPublished
        {
            get { return ContentItem.VersionRecord != null && ContentItem.VersionRecord.Published; }
        }

        public bool HasDraft
        {
            get
            {
                return (
                           (ContentItem.VersionRecord != null) && (
                               (ContentItem.VersionRecord.Published == false) ||
                               (ContentItem.VersionRecord.Published && ContentItem.VersionRecord.Latest == false)));
            }
        }

        public bool HasPublished
        {
            get
            {
                return IsPublished || ContentItem.ContentManager.Get(Id, VersionOptions.Published) != null;
            }
        }

        public DateTime? PublishedUtc
        {
            get { return this.As<ICommonPart>().PublishedUtc; }
        }

        //public IEnumerable<CasePostStateRecord> CaseStates
        //{
        //    get
        //    {
        //        return getCaseStates(ContentItem.Id);
        //    }
        //}

        //private IEnumerable<CasePostStateRecord> getCaseStates(int id) {
        // ICasePostService casePostService=null ;
        //    //casePostService= ContainerBuilder.
        //    return casePostService.GetCaseStates(id);
                   
        //}
    }
}