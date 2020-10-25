﻿using AutoMapper;
using PDCore.Common.Context.IContext;
using PDCore.Interfaces;
using PDCore.Repositories.IRepo;
using System;
using System.Collections.Generic;

namespace PDCore.Common.Factories.Fac.Repository
{
    /// <summary>
    /// Provides an <see cref="IRepository{T}"/> for a client request.
    /// </summary>
    /// <remarks>
    /// Caches repositories of a given type so that repositories are only created once per provider.
    /// Code Camper creates a new provider per client request.
    /// </remarks>
    public class RepositoryProvider : IRepositoryProvider
    {
        /// <summary>
        /// Get the dictionary of repository objects, keyed by repository type.
        /// </summary>
        /// <remarks>
        /// Caller must know how to cast the repository object to a useful type.
        /// <p>This is an extension point. You can register fully made repositories here
        /// and they will be used instead of the ones this provider would otherwise create.</p>
        /// </remarks>
        protected Dictionary<Type, object> Repositories { get; private set; }

        /// <summary>
        /// The <see cref="RepositoryFactories"/> with which to create a new repository.
        /// </summary>
        /// <remarks>
        /// Should be initialized by constructor injection
        /// </remarks>
        private readonly RepositoryFactories _repositoryFactories;
        private readonly ILogger logger;
        private readonly IMapper mapper;

        public IEntityFrameworkDbContext DbContext { get; set; }

        public RepositoryProvider(RepositoryFactories repositoryFactories, ILogger logger, IMapper mapper)
        {
            _repositoryFactories = repositoryFactories;

            this.logger = logger;
            this.mapper = mapper;
            Repositories = new Dictionary<Type, object>();
        }

        ///// <summary>
        ///// Get and set the <see cref="IEntityFrameworkDbContext"/> with which to initialize a repository
        ///// if one must be created.
        ///// </summary>
        //public IEntityFrameworkDbContext DbContext { get; set; }
        ///// <summary>
        ///// Get and set the <see cref="ILogger"/> with which to initialize a repository
        ///// if one must be created.
        ///// </summary>
        //public ILogger Logger { get; set; }

        /// <summary>
        /// Get or create-and-cache the default <see cref="ISqlRepositoryEntityFramework{T}"/> for an entity of type T.
        /// </summary>
        /// <typeparam name="T">
        /// Root entity type of the <see cref="ISqlRepositoryEntityFramework{T}"/>.
        /// </typeparam>
        /// <remarks>
        /// If can't find repository in cache, use a factory to create one.
        /// </remarks>
        public ISqlRepositoryEntityFramework<T> GetRepositoryForEntityType<T>() where T : class, IModificationHistory
        {
            return GetRepository<ISqlRepositoryEntityFramework<T>>(_repositoryFactories.GetRepositoryFactoryForEntityType<T>());
        }

        public ISqlRepositoryEntityFrameworkConnected<T> GetRepositoryForEntityTypeConnected<T>() where T : class, IModificationHistory, new()
        {
            return GetRepository<ISqlRepositoryEntityFrameworkConnected<T>>(_repositoryFactories.GetRepositoryFactoryForEntityTypeConnected<T>());
        }

        public ISqlRepositoryEntityFrameworkDisconnected<T> GetRepositoryForEntityTypeDisconnected<T>() where T : class, IModificationHistory
        {
            return GetRepository<ISqlRepositoryEntityFrameworkDisconnected<T>>(_repositoryFactories.GetRepositoryFactoryForEntityTypeDisconnected<T>());
        }

        /// <summary>
        /// Get or create-and-cache a repository of type T.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the repository, typically a custom repository interface.
        /// </typeparam>
        /// <param name="factory">
        /// An optional repository creation function that takes a DbContext argument
        /// and returns a repository of T. Used if the repository must be created and
        /// caller wants to specify the specific factory to use rather than one
        /// of the injected <see cref="RepositoryFactories"/>.
        /// </param>
        /// <remarks>
        /// Looks for the requested repository in its cache, returning if found.
        /// If not found, tries to make one using <see cref="MakeRepository{T}"/>.
        /// </remarks>
        public virtual T GetRepository<T>(Func<IEntityFrameworkDbContext, ILogger, IMapper, object> factory = null) where T : class
        {
            // Look for T dictionary cache under typeof(T).
            Repositories.TryGetValue(typeof(T), out object repoObj);

            if (repoObj != null)
            {
                return (T)repoObj;
            }

            // Not found or null; make one, add to dictionary cache, and return it.
            return MakeRepository<T>(factory, DbContext, logger, mapper);
        }

        /// <summary>Make a repository of type T.</summary>
        /// <typeparam name="T">Type of repository to make.</typeparam>
        /// <param name="dbContext">
        /// The <see cref="DbContext"/> with which to initialize the repository.
        /// </param>        
        /// <param name="factory">
        /// Factory with <see cref="DbContext"/> argument. Used to make the repository.
        /// If null, gets factory from <see cref="_repositoryFactories"/>.
        /// </param>
        /// <returns></returns>
        protected virtual T MakeRepository<T>(Func<IEntityFrameworkDbContext, ILogger, IMapper, object> factory, IEntityFrameworkDbContext dbContext, ILogger logger, IMapper mapper)
        {
            var f = factory ?? _repositoryFactories.GetRepositoryFactory<T>() ?? _repositoryFactories.GetDefaultRepositoryFactory<T>();

            if (f == null)
            {
                throw new NotImplementedException("No factory for repository type, " + typeof(T).FullName);
            }

            var repo = (T)f(dbContext, logger, mapper);

            Repositories[typeof(T)] = repo;

            return repo;
        }

        /// <summary>
        /// Set the repository for type T that this provider should return.
        /// </summary>
        /// <remarks>
        /// Plug in a custom repository if you don't want this provider to create one.
        /// Useful in testing and when developing without a backend
        /// implementation of the object returned by a repository of type T.
        /// </remarks>
        public void SetRepository<T>(T repository)
        {
            Repositories[typeof(T)] = repository;
        }
    }
}
