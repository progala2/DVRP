using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UCCTaskSolver;

namespace _15pl04.Ucc.Commons
{
    public static class TaskSolversLoader
    {
        /// <summary>
        /// Gets dictionary with names of solvable problems as keys and proper TaskSolvers as values.
        /// </summary>
        /// <param name="taskSolversDirectoryRelativePath">The relative path of directory to search.</param>
        /// <returns>A dictionary with names of solvable problems as keys and proper TaskSolvers as values.</returns>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        public static ReadOnlyDictionary<string, Type> GetTaskSolversFromRelativePath(string taskSolversDirectoryRelativePath)
        {            
            var taskSolversDirectoryPath = Path.GetFullPath(Directory.GetCurrentDirectory() + taskSolversDirectoryRelativePath);
            return GetTaskSolversFromPath(taskSolversDirectoryPath);
        }

        /// <summary>
        /// Gets dictionary with names of solvable problems as keys and proper TaskSolvers as values.
        /// </summary>
        /// <param name="taskSolversDirectoryPath">The path of directory to search.</param>
        /// <returns>A dictionary with names of solvable problems as keys and proper TaskSolvers as values.</returns>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        public static ReadOnlyDictionary<string, Type> GetTaskSolversFromPath(string taskSolversDirectoryPath)
        {
            var dictionary = new Dictionary<string, Type>();

            // add <key,value> pairs based on types derived from TaskSolver in *.dll files
            var libraries = Directory.GetFiles(taskSolversDirectoryPath, "*.dll");
            var typeOfTaskSolver = typeof(TaskSolver);
            foreach (var file in libraries)
            {
                Assembly assembly = Assembly.LoadFile(file);
                var taskSolversTypes = assembly.GetTypes().Where(t => typeOfTaskSolver.IsAssignableFrom(t) && !t.IsAbstract);
                foreach (var taskSolverType in taskSolversTypes)
                {
                    /* NOTE: there's a possibilty of throwing an Exception because TaskSolver has
                     * only one constructor which takes byte[] as parameter and behavior of it can
                     * vary on specific implementation */
                    var taskSolver = (TaskSolver)Activator.CreateInstance(taskSolverType, new byte[0]);
                    dictionary.Add(taskSolver.Name, taskSolverType);
                }
            }

            var readOnlyDictionary = new ReadOnlyDictionary<string, Type>(dictionary);
            return readOnlyDictionary;
        }
    }
}
