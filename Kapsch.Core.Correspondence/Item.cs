using Kapsch.Core.Data;
using Kapsch.Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Correspondence
{
    public class Item
    {
        private ICorrespondent target;
        private ICorrespondent source;
        private IPayload payload;
        private CorrespondenceRoute route;
        private string internalReference;

        private Item(string internalReference, ICorrespondent source, ICorrespondent target, CorrespondenceRoute correspondenceRoute, IPayload payload)
        {
            this.internalReference = internalReference;
            this.source = source;
            this.target = target;
            this.payload = payload;
        }

        public static Item Initiate(DataContext dataContext, string internalReference, ICorrespondent source, ICorrespondent target, Router router, IPayload payload, bool wait = true)
        {
            if (source == null)
                throw new Exception("source argument can not be null.");

            if (target == null)
                throw new Exception("target argument can not be null.");

            if (payload == null)
                throw new Exception("payload argument can not be null.");

            if (router == null)
                router = Router.GetRoute(source, target, payload.CorrespondenceType);

            if (router == null)
                throw new Exception("Unable to compute route for correspondence item.");
 
            CorrespondenceItem correspondenceItem = new CorrespondenceItem();
            correspondenceItem.Context = payload.CorrespondenceContext;
            correspondenceItem.CorrespondenceType = payload.CorrespondenceType;
            correspondenceItem.CreatedTimestamp = DateTime.Now;
            correspondenceItem.Status = CorrespondenceItemStatus.Created;
            correspondenceItem.StatusTimestamp = correspondenceItem.CreatedTimestamp;
            correspondenceItem.TargetEntityID = target.ID;
            correspondenceItem.TargetEntityType = GetEntityType(target);
            correspondenceItem.SourceEntityID = source.ID;
            correspondenceItem.SourceEntityType = GetEntityType(source);
            correspondenceItem.SubType = payload.SubType;
            correspondenceItem.InternalReference = internalReference;
            correspondenceItem.Value = "RFU";            
            dataContext.CorrespondenceItems.Add(correspondenceItem);

            CorrespondenceRoute correspondenceRoute = new CorrespondenceRoute();
            correspondenceRoute.Source = router.Source;
            correspondenceRoute.Target = router.Target;
            correspondenceRoute.CorrespondenceItem = correspondenceItem;
            dataContext.CorrespondenceRoutes.Add(correspondenceRoute);

            if (payload.CorrespondenceType == CorrespondenceType.Email)
            {
                // TODO
            }
            else if (payload.CorrespondenceType == CorrespondenceType.Sms)
            {
                if (!(payload is SmsPayload))
                    throw new Exception("Expected sms payload.");

                CorrespondenceSmsPayload correspondenceSmsPayload = new CorrespondenceSmsPayload();
                correspondenceSmsPayload.CorrespondenceItem = correspondenceItem;
                correspondenceSmsPayload.Message = ((SmsPayload)payload).Message;
                dataContext.CorrespondenceSmsPayloads.Add(correspondenceSmsPayload);
            }

            dataContext.SaveChanges(); 
            
            Processor.Process(correspondenceItem.ID, wait);

            return new Item(correspondenceItem.InternalReference, source, target, correspondenceRoute, payload);
        }

        public ICorrespondent Target
        {
            get
            {
                return this.target;
            }
        }

        public ICorrespondent Source
        {
            get
            {
                return this.source;
            }
        }

        public string InternalReference
        {
            get
            {
                return this.internalReference;
            }
        }

        public static bool CanRoute(ICorrespondent source, ICorrespondent target, IPayload payload)
        {
            if (source == null)
                throw new Exception("source argument can not be null.");

            if (target == null)
                throw new Exception("target argument can not be null.");

            if (payload == null)
                throw new Exception("payload argument can not be null.");

            return Router.CanRoute(source, target, payload);
        }

        public static EntityType GetEntityType(ICorrespondent correspondent)
        {
            if (correspondent is Person)
                return EntityType.Person;

            if (correspondent is Company)
                return EntityType.Company;

            if (correspondent is User)
                return EntityType.User;

            throw new Exception("Correspondent has unknown type.");
        }
    }
}
