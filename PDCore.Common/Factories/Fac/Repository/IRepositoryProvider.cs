﻿using AutoMapper;
using PDCore.Common.Context.IContext;
using PDCore.Interfaces;
using PDCore.Repositories.IRepo;
using System;

namespace PDCore.Common.Factories.Fac.Repository
{
    /// <summary>
    /// Interface for a class that can provide repositories by type.
    /// The class may create the repositories dynamically if it is unable
    /// to find one in its cache of repositories.
    /// </summary>
    /// <remarks>
    /// Repositories created by this provider tend to require a <see cref="IEntityFrameworkDbContext"/> and <see cref="ILogger"/>
    /// to retrieve data.
    /// </remarks>
    public interface IRepositoryProvider
    {
        /// <summary>
        /// Get an <see cref="{T}"/> for entity type, T.
        /// </summary>
        /// <typeparam name="T">
        /// Root entity type of the <see cref="ISqlRepositoryEntityFramework{T}"/>.
        /// </typeparam>
        ISqlRepositoryEntityFramework<T> GetRepositoryForEntityType<T>() where T : class, IModificationHistory;
        ISqlRepositoryEntityFrameworkConnected<T> GetRepositoryForEntityTypeConnected<T>() where T : class, IModificationHistory, new();
        ISqlRepositoryEntityFrameworkDisconnected<T> GetRepositoryForEntityTypeDisconnected<T>() where T : class, IModificationHistory;

        /// <summary>
        /// Get a repository of type T.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the repository, typically a custom repository interface.
        /// </typeparam>
        /// <param name="factory">
        /// An optional repository creation function that takes a <see cref="IEntityFrameworkDbContext"/> and <see cref="ILogger"/>
        /// and returns a repository of T. Used if the repository must be created.
        /// </param>
        /// <remarks>
        /// Looks for the requested repository in its cache, returning if found.
        /// If not found, tries to make one with the factory, fallingback to 
        /// a default factory if the factory parameter is null.
        /// </remarks>
        T GetRepository<T>(Func<IEntityFrameworkDbContext, ILogger, IMapper, object> factory = null) where T : class;


        /// <summary>
        /// Set the repository to return from this provider.
        /// </summary>
        /// <remarks>
        /// Set a repository if you don't want this provider to create one.
        /// Useful in testing and when developing without a backend
        /// implementation of the object returned by a repository of type T.
        /// </remarks>
        void SetRepository<T>(T repository);

        IEntityFrameworkDbContext DbContext { get; set; }
    }
}
