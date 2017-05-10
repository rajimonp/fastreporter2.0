using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Tasks.Scheduling;
using LawBookCases.Module.Models;
using LawBookCases.Module.Services;


namespace LawBookCases.Module.Services {
    public class CasePostService : ICasePostService {
        private readonly IContentManager _contentManager;
        private readonly IRepository<CasePartArchiveRecord> _caseArchiveRepository;
        private readonly IPublishingTaskManager _publishingTaskManager;
        private readonly IRepository<CasePostAttribRecord> _caseAttribute;
        private readonly IRepository<CasePostStateRecord> _casePostStates;

        public CasePostService(
            IContentManager contentManager, 
            IRepository<CasePartArchiveRecord> caseArchiveRepository, 
            IPublishingTaskManager publishingTaskManager, IRepository<CasePostAttribRecord> caseAttribute, IRepository<CasePostStateRecord> casePostStates) {
            _contentManager = contentManager;
            _caseArchiveRepository = caseArchiveRepository;
            _publishingTaskManager = publishingTaskManager;
            _caseAttribute = caseAttribute;
            _casePostStates = casePostStates;
    }

        public CasePostPart Get(int id) {
            return Get(id, VersionOptions.Published);
        }
        
        public CasePostPart Get(int id, VersionOptions versionOptions) {
            _contentManager.Query(versionOptions, "CasePost")
                .Join<CasePostAttribRecord>().Where(attrib => attrib.ContentItemRecord.Id==id);
         //   var postPart=_contentManager.Get<CasePostPart>(id, versionOptions);
         //   var attribPart = _contentManager.Query(versionOptions, typeof(CasePostAttribPart).Name).Where(attrib=>attrib.ContentItemRecord.Id==)
                //Where(attrib=>attrib.ContentItemRecord.Id== postPart.Id);
          //      var 

            return _contentManager.Get<CasePostPart>(id, versionOptions);
        }

        public IEnumerable<CasePostPart> Get(CasePart casePart) {
            return Get(casePart, VersionOptions.Published);
        }

        public IEnumerable<CasePostPart> Get(CasePart casePart, VersionOptions versionOptions) {
            return GetCaseQuery(casePart, versionOptions).List().Select(ci => ci.As<CasePostPart>());
        }

        public IEnumerable<CasePostPart> Get(CasePart casePart, int skip, int count) {
            return Get(casePart, skip, count, VersionOptions.Published)
                  .OrderByDescending(attrib => attrib.Get<CasePostAttribPart>().CaseYear)
              //    .OrderByDescending(attrib => attrib.Get<CasePostAttribPart>().CaseNumber)
               ;
        }

        public IEnumerable<CasePostPart> Get(CasePart casePart, int skip, int count, VersionOptions versionOptions) {
            return GetCaseQuery(casePart, versionOptions)
                    .Slice(skip, count)
                    .ToList()
                    .Select(ci => ci.As<CasePostPart>());
        }

        public int PostCount(CasePart casePart) {
            return PostCount(casePart, VersionOptions.Published);
        }

        public int PostCount(CasePart casePart, VersionOptions versionOptions) {
            return _contentManager.Query(versionOptions, "CasePost")
                .Join<CommonPartRecord>().Where(
                    cr => cr.Container.Id == casePart.Id)
                .Count();
        }

        public IEnumerable<CasePostPart> Get(CasePart cAsePart, ArchiveData archiveData) {
            var query = GetCaseQuery(cAsePart, VersionOptions.Published);

           /* if (archiveData.Day > 0) {
                var dayDate = new DateTime(archiveData.Year, archiveData.Month, archiveData.Day);

                query = query.Where(cr => cr.CreatedUtc >= dayDate && cr.CreatedUtc < dayDate.AddDays(1));
            }
            else if (archiveData.Month > 0)
            {
                var monthDate = new DateTime(archiveData.Year, archiveData.Month, 1);

                query = query.Where(cr => cr.CreatedUtc >= monthDate && cr.CreatedUtc < monthDate.AddMonths(1));
            }
            else {
                var yearDate = new DateTime(archiveData.Year, 1, 1);

                query = query.Where(cr => cr.CreatedUtc >= yearDate && cr.CreatedUtc < yearDate.AddYears(1));
            }
            */
            return query.List().Select(ci => ci.As<CasePostPart>());
        }

        public IEnumerable<KeyValuePair<ArchiveData, int>> GetArchives(CasePart cAsePart) {
            var query = 
                from bar in _caseArchiveRepository.Table
                where bar.CasePart.Id == cAsePart.Id
                orderby bar.Year descending, bar.Month descending
                select bar;

            return
                query.ToList().Select(
                    bar =>
                    new KeyValuePair<ArchiveData, int>(new ArchiveData(string.Format("{0}/{1}", bar.Year, bar.Month)),
                                                       bar.PostCount));
        }

        public void Delete(CasePostPart cAsePostPart) {
            _publishingTaskManager.DeleteTasks(cAsePostPart.ContentItem);
            _contentManager.Remove(cAsePostPart.ContentItem);
        }

        public void Publish(CasePostPart cAsePostPart) {
            _publishingTaskManager.DeleteTasks(cAsePostPart.ContentItem);
            _contentManager.Publish(cAsePostPart.ContentItem);
        }

        public void Publish(CasePostPart cAsePostPart, DateTime scheduledPublishUtc) {
            _publishingTaskManager.Publish(cAsePostPart.ContentItem, scheduledPublishUtc);
        }

        public void Unpublish(CasePostPart cAsePostPart) {
            _contentManager.Unpublish(cAsePostPart.ContentItem);
        }

        public void PuAcquierblish(CasePostPart cAsePostPart)
        {
            
            _contentManager.Publish(cAsePostPart.ContentItem);
        }
        public DateTime? GetScheduledPublishUtc(CasePostPart cAsePostPart) {
            var task = _publishingTaskManager.GetPublishTask(cAsePostPart.ContentItem);
            return (task == null ? null : task.ScheduledUtc);
        }

        private IContentQuery<ContentItem, CommonPartRecord> GetCaseQuery(CasePart cAse, VersionOptions versionOptions) {
            
                var cnt=_contentManager.Query(versionOptions, "CasePost")
                .Join<CommonPartRecord>().Where(
                    cr => cr.Container.Id == cAse.Id).OrderByDescending(cr => cr.CreatedUtc)
                    ;

                     

            return cnt;
        }
            
        public int GetNextCaseNumber(int Year)
        {

            int caseNumber = _caseAttribute.Table.ToList().Where(y => y.CaseYear == Year).Max(num => num.CaseNumber);
            
            return caseNumber +1;
        }

       
        public void UpdaetCasePostAttributes(CasePostAttribRecord attribute)
        {
           
            _caseAttribute.Update(attribute);
          

        }

        public IEnumerable<int> GetCaseYears()
        {
            var casNum=  _caseAttribute.Table.Select(c => c.CaseYear).Distinct().ToList();

         //   var caseYears = (from year in _caseAttribute. select year).GroupBy(c => new { c.CaseYear }).Select(g => g.FirstOrDefault()).ToList().GetEnumerator();
            return casNum;
        }

        public IEnumerable<CasePostPart> Get(CasePart cAsePart, int year, int skip, int pageSize)
        {

             IEnumerable<CasePostPart> cnt2= Get(cAsePart, skip, pageSize, VersionOptions.Published)
               .Where(attrib => attrib.Get<CasePostAttribPart>().CaseYear == year)
               .OrderByDescending(attrib=> attrib.Get<CasePostAttribPart>().CaseYear)
               .OrderByDescending(attrib => attrib.Get<CasePostAttribPart>().CaseNumber)
               ;

           return cnt2;
        }

        //public CasePostPart Get(CasePart cAsePart, int year, int casEnumber)
        //{
        //    var cnt = _contentManager.Query(VersionOptions.Published, "CasePost")
        //     .Join<CasePostAttribRecord>().Where(
        //         cr => cr.CaseNumber == casEnumber).Where(
        //        cr=>cr.CaseYear==year)     ;

        //    CasePostPart part = cnt.List().FirstOrDefault().Get<CasePostPart>();


        //    return part;
        //}

        public IEnumerable<CasePostStateRecord> GetCaseStates(int id)
        {
            return _casePostStates.Table.ToList().Where(contentid => contentid.CasePostPart_id == id);
        }

         public void Acquier(CasePostStateRecord rec)
        {

            _casePostStates.Create(rec);
        }

        public void GetCurrentState(CasePostPart postpart)
        {
           CasePostAttribPart attri = postpart.Get<CasePostAttribPart>();
            attri.CurrentPostState = _casePostStates.Table.Where(rec => rec.CasePostPart_id == postpart.Id)
                                        .OrderByDescending(rec=>rec.CasePostStateUtc)
                                        .FirstOrDefault();

        }

        public bool isInterAcquiered(int id)
        {
            // _casePostStates.Table.Where(rec => rec.CasePostPart_id == id);
            //   var p=  from acq  in _casePostStates.Table
            //          where(acq=>acq.)

            return true;
          //  throw new NotImplementedException();
        }
    }
}