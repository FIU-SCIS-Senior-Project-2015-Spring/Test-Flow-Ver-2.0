using DataStore.EntityData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    public class CollectionHelper
    {
        public TF_User User { get; set; }

        public CollectionHelper(string Username)
        {
            using (var context = new testflowEntities())
            {
                User = (from u in context.TF_User where u.Username.Equals(Username) select u).FirstOrDefault();
            }
        }
        public void CreateCollection(Collection collection, int type)
        {
            TF_Collections dbCollection = new TF_Collections();
            dbCollection.Host = collection.Host.ToString();
            dbCollection.Name = collection.Name;
            dbCollection.Type_Id = type;

            using (var context = new testflowEntities())
            {
                context.TF_Collections.Add(dbCollection);
                try
                {
                    context.SaveChanges();
                    CreatePermission(dbCollection.Collection_Id);
                }
                catch (Exception e)
                {

                }
            }
        }

        public void EditCollection(Collection collection)
        {
            using (var context = new testflowEntities())
            {
                var dbCollection = context.TF_Collections.Find(collection.Id);
                dbCollection.Name = collection.Name;
                dbCollection.Host = collection.Host;
                try
                {
                    context.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }
        }

        public Collection GetCollection(int id)
        {
            using (var context = new testflowEntities())
            {
                Collection collection = new Collection();
                var dbCollection = context.TF_Collections.Find(id);
                collection.Name = dbCollection.Name;
                collection.Host = dbCollection.Host;
                collection.Id = dbCollection.Collection_Id;

                return collection;
            }
        }

        public List<Collection> GetCollections()
        {
            using (var context = new testflowEntities())
            {
                var permissions = from p in context.TF_User_Permissions where p.User_Id == User.User_Id select p.Collection_Id;
                var dbCollections = context.TF_Collections.Where(p => permissions.Contains(p.Collection_Id));
                List<Collection> collections = new List<Collection>();

                foreach (TF_Collections c in dbCollections)
                {
                    Collection collection = new Collection();
                    collection.Id = c.Collection_Id;
                    collection.Name = c.Name;
                    collection.Host = c.Host;
                    collections.Add(collection);
                }

                return collections;
            }
        }

        private void CreatePermission(int CollectionId)
        {
            using (var context = new testflowEntities())
            {
                var permissions = from p in context.TF_User_Permissions where p.Collection_Id == CollectionId select p;
                if (permissions.Count() <= 0)
                {
                    TF_User_Permissions permission = new TF_User_Permissions();
                    permission.Collection_Id = CollectionId;
                    permission.User_Id = User.User_Id;
                    context.TF_User_Permissions.Add(permission);
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception e)
                    {

                    }

                }
            }
        }
    }
}
