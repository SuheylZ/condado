// --------------------------------------------------------------------------
// Application:  SQ Sales Tool
// 
// Project:      SalesTool.DataAccess
// 
// Description: This application is created for Condado Group. the application 
//              is accessible from the condado-02 (QA site)
//              
// 
// Created By:   SZ
// Created On:   12/12/2012
// 
// --------------------------------------------------------------------------
// 
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

/*
 * SZ [Dec 12, 2012] This is an experiment of generic repository pattern. 
 * 
 * .:: WARNING ::.
 * 
 * DO NOT use it unless properly tested
 * 
 */

 namespace SalesTool.DataAccess
{
     //public abstract class GenericActions<T, S>
     //       where T : EntityObject, new()
     //{
     //    protected DBEngine engine = null;

     //    internal BaseActions(DBEngine reng)
     //    {
     //        engine = reng;
     //    }


     //    public virtual void Add(T item)
     //    {
     //        engine.entities.AddObject(item.EntityKey.EntitySetName, item);
     //        engine.Save();
     //    }
     //    public virtual void Update(T item)
     //    {
     //        engine.Save();
     //    }

     //    public virtual void Delete(T item)
     //    {
     //        engine.entities.DeleteObject(item);
     //        engine.Save();
     //    }
     //    public virtual void Delete(S key)
     //    {
     //        Delete(Get(key));
     //    }

     //    public abstract T Get(S id);
     //    public abstract IQueryable<T> All { get; }
     //}

     public class BaseActions
     {
         protected readonly DBEngine E=null;

         public BaseActions(DBEngine rengine)
         {
             E = rengine;
         }
         protected DBEngine Engine { get { return E; } }
             
     }


     //public class GenericActions<TEntity> 
     //    where TEntity : System.Data.Objects.DataClasses.EntityObject
     //{
     //    DBEngine E;
     //    //TEntity data;

     //    public GenericActions(DBEngine engine)
     //    {
     //        E= engine;
     //    }

     //    //public virtual IEnumerable<TEntity> Get(
     //    //    Expression<Func<TEntity, bool>> filter = null,
     //    //    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
     //    //    string includeProperties = "")
     //    //{
     //    //    IQueryable<TEntity> query = data;

     //    //    if (filter != null)
     //    //    {
     //    //        query = query.Where(filter);
     //    //    }

     //    //    foreach (var includeProperty in includeProperties.Split
     //    //        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
     //    //    {
     //    //        query = query.Include(includeProperty);
     //    //    }

     //    //    if (orderBy != null)
     //    //    {
     //    //        return orderBy(query).ToList();
     //    //    }
     //    //    else
     //    //    {
     //    //        return query.ToList();
     //    //    }
     //    //}

     //    //public virtual TEntity GetByID(object id)
     //    //{
     //    //    return data.Find(id);
     //    //}

     //    //public virtual void Insert(TEntity entity)
     //    //{
     //    //    data.Add(entity);
     //    //}

     //    //public virtual void Delete(object id)
     //    //{
     //    //    TEntity entityToDelete = data.Find(id);
     //    //    Delete(entityToDelete);
     //    //}

     //    public virtual void Delete(TEntity entity)
     //    {
     //        E.Admin.DeleteObject(entity);
     //    }

     //    public virtual void Add(TEntity entity)
     //    {
     //        E.Admin.AddObject(entity.EntityKey.EntitySetName, entity);
     //    }

     //    public virtual TEntity Get(object id)
     //    {
     //        EntityKey  key = E.Admin.CreateEntityKey(data.EntityKey.EntitySetName, data);
     //        return E.Admin.GetObjectByKey(key) as TEntity;
     //    }
     //}


}
