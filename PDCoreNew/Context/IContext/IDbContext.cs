﻿using PDCoreNew.Interfaces;
using System;

namespace PDCoreNew.Context.IContext
{
    /// <summary>
    /// Interfejs zawierający metody przydatne dla kontekstu bazy danych bez brania pod uwagę specyficznych uwarunkowań implementacyjnych.
    /// Obiekt implementujący ten interfejs musi być disposable
    /// </summary>
    public interface IDbContext : IDisposable
    {
        /// <summary>
        /// Określenie czy logowanie informacji o zapytaniach jest aktualnie aktywne
        /// </summary>
        bool IsLoggingEnabled { get; }

        /// <summary>
        /// Ustawienie czy logowanie informacji o zapytaniach ma być aktywne
        /// </summary>
        /// <param name="res">Flaga czy logowanie informacji o zapytaniach ma być aktywne</param>
        void SetLogging(bool input, ILogger logger);
    }
}
