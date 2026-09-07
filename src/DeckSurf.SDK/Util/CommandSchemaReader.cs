// Copyright (c) Den
// Den licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DeckSurf.SDK.Interfaces;
using DeckSurf.SDK.Models;

namespace DeckSurf.SDK.Util
{
    /// <summary>
    /// Reads the declared parameter schema of a DeckSurf command from its
    /// <see cref="CommandParameterAttribute"/> annotations, without requiring the command
    /// to be instantiated.
    /// </summary>
    public static class CommandSchemaReader
    {
        /// <summary>
        /// Gets the declared parameters for a command type, sorted by
        /// <see cref="CommandParameterAttribute.Order"/> and then by key. When a parameter key
        /// is declared on both a base class and a derived class, the derived class declaration wins.
        /// </summary>
        /// <param name="commandType">The command type to inspect.</param>
        /// <returns>The declared parameters, or an empty list for commands without annotations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="commandType"/> is null.</exception>
        public static IReadOnlyList<CommandParameterAttribute> GetParameters(Type commandType)
        {
            ArgumentNullException.ThrowIfNull(commandType);

            var parameters = new Dictionary<string, CommandParameterAttribute>(StringComparer.OrdinalIgnoreCase);

            // Walk from the most derived type up so that derived declarations take
            // precedence over base class declarations for the same key.
            for (var type = commandType; type != null && type != typeof(object); type = type.BaseType)
            {
                foreach (var attribute in type.GetCustomAttributes<CommandParameterAttribute>(inherit: false))
                {
                    if (!parameters.ContainsKey(attribute.Key))
                    {
                        parameters.Add(attribute.Key, attribute);
                    }
                }
            }

            return [.. parameters.Values
                .OrderBy(p => p.Order)
                .ThenBy(p => p.Key, StringComparer.OrdinalIgnoreCase)];
        }

        /// <summary>
        /// Gets the declared parameters for a command instance.
        /// </summary>
        /// <param name="command">The command instance to inspect.</param>
        /// <returns>The declared parameters, or an empty list for commands without annotations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
        public static IReadOnlyList<CommandParameterAttribute> GetParameters(IDeckSurfCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);

            return GetParameters(command.GetType());
        }
    }
}
