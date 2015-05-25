using System;
using System.Collections.Generic;

namespace LawSoft.Ioc.Interface
{

    /// <summary>
    /// The delegate define how to create object
    /// </summary>
    /// <typeparam name="TClass">The type that must be used to create instances.</typeparam>
    /// <returns></returns>
    public delegate TClass Creator<out TClass>();

    /// <summary>
    /// A very simple IOC container with basic functionality needed to register and resolve
    /// instances. 
    /// </summary>
    public interface ISimpleIoc
    {
        /// <summary>
        /// Checks whether the current type is already registered
        /// </summary>
        /// <typeparam name="T">The type whether registered</typeparam>
        /// <returns></returns>
        bool IsRegistered<T>();

        /// <summary>
        /// Checks instance of a given class is already created in the container.
        /// </summary>
        /// <typeparam name="T">The class that is queried.</typeparam>
        /// <returns>True if at least on instance of the class is already created, false otherwise.</returns>
        bool ContainsCreated<T>();

        #region [ Register ]
        /// <summary>
        /// Registers a given instance.
        /// </summary>
        /// <param name="instance">The register instance</param>
        /// <param name="type">The instance must be inherit this type.</param>
        /// <returns></returns>
        ISimpleIoc Register(object instance, Type type);

        /// <summary>
        /// Registers a given instance.
        /// </summary>
        /// <typeparam name="TClass">The register instance.</typeparam>
        ISimpleIoc Register<TClass>(TClass instance) where TClass : class;

        /// <summary>
        /// Registers a given instance for a given type.
        /// </summary>
        /// <typeparam name="TClass">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        ISimpleIoc Register<TClass>(Creator<TClass> factory) where TClass : class;

        /// <summary>
        /// Registers a given instance for a given type with the possibility for immediate
        /// creation of the instance.
        /// </summary>
        /// <typeparam name="TClass">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        /// <param name="createInstanceImmediately">If true, forces the creation of the default
        /// instance of the provided class.</param>
        ISimpleIoc Register<TClass>(Creator<TClass> factory, bool createInstanceImmediately) where TClass : class;

        /// <summary>
        /// Registers a given type for a given interface.
        /// </summary>
        /// <typeparam name="TInterface">The interface for which instances will be resolved.</typeparam>
        /// <typeparam name="TClass">The type that must be used to create instances.</typeparam>
        ISimpleIoc Register<TInterface, TClass>()
            where TClass : class, TInterface
            where TInterface : class;

        /// <summary>
        /// Registers a given type for a given interface with the possibility for immediate
        /// creation of the instance.
        /// </summary>
        /// <typeparam name="TInterface">The interface for which instances will be resolved.</typeparam>
        /// <typeparam name="TClass">The type that must be used to create instances.</typeparam>
        /// <param name="createInstanceImmediately">If true, forces the creation of the default
        /// instance of the provided class.</param>
        ISimpleIoc Register<TInterface, TClass>(bool createInstanceImmediately)
            where TClass : class, TInterface
            where TInterface : class;

        #endregion

        #region [ Unregister ]
        /// <summary>
        /// Unregisters a class from the cache and removes all the previously
        /// created instances.
        /// </summary>
        /// <typeparam name="TClass">The class that must be removed.</typeparam>
        void Unregister<TClass>()
            where TClass : class; 
        #endregion

        #region [ GetInstance ]

        /// <summary>
        /// Provides a way to get an instance of a given type. If no instance had been instantiated 
        /// before, a new instance will be created. If an instance had already
        /// been created, that same instance will be returned.
        /// </summary>
        /// <returns>An instance of the given type.</returns>
        TClass GetInstance<TClass>();

        /// <summary>
        /// Provides a way to get an instance of a given type. If no instance had been instantiated 
        /// before, a new instance will be created. If an instance had already
        /// been created, that same instance will be returned.
        /// </summary>
        /// <param name="serviceType">The class of which an instance
        /// must be returned.</param>
        /// <returns>An instance of the given type.</returns>
        object GetInstance(Type serviceType);

        /// <summary>
        /// Looks up an object within the container based on the runtime name of the Type
        /// </summary>
        /// <param name="typeName">The namer of the contract to lookup</param>
        /// <returns>An instance of TContract or null if not found</returns>
        object GetInstance(string typeName);

        #endregion

        #region [ GetAllInstances ]
        /// <summary>
        /// Provides a way to get all the created instances available in the
        /// cache. Calling this method auto-creates 
        /// instances for all registered classes.
        /// </summary>
        /// <returns>All the instances of registered.</returns>
        IEnumerable<object> GetAllInstances();
        #endregion

        /// <summary>
        /// Provides a way to get all the created instance of a given base type available in the
        /// cache. Calling this method auto-creates instances for all registed classes for derived
        /// given class
        /// </summary>
        /// <typeparam name="TBaseClass"></typeparam>
        /// <returns></returns>
        IEnumerable<TBaseClass> GetAllInstancesOnBase<TBaseClass>() where TBaseClass : class;

        /// <summary>
        /// Resets the instance in its original states. This deletes all the
        /// registrations.
        /// </summary>
        void Reset();
    }
}
