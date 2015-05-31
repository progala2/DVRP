using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using _15pl04.Ucc.Commons.Exceptions;
using UCCTaskSolver;

namespace _15pl04.Ucc.Commons
{
    /// <summary>
    /// Class providing loading task solvers from directory.
    /// </summary>
    public static class TaskSolverLoader
    {
        /// <summary>
        /// Gets dictionary with names of solvable problems as keys and corresponding Task Solvers as values.
        /// </summary>
        /// <param name="taskSolversDirectoryRelativePath">The relative path of directory to search.</param>
        /// <returns>Read-only dictionary with names of solvable problems as keys and corresponding Task Solvers as values.</returns>
        /// <exception cref="_15pl04.Ucc.Commons.Exceptions.TaskSolverLoadingException">Thrown when exception occured
        /// during loading task solvers.</exception>
        public static ReadOnlyDictionary<string, Type> GetTaskSolversFromRelativePath(
            string taskSolversDirectoryRelativePath)
        {
            var taskSolversDirectoryPath =
                Path.GetFullPath(Directory.GetCurrentDirectory() + taskSolversDirectoryRelativePath);
            return GetTaskSolversFromPath(taskSolversDirectoryPath);
        }

        /// <summary>
        /// Gets dictionary with names of solvable problems as keys and corresponding Task Solvers as values.
        /// </summary>
        /// <param name="taskSolversDirectoryPath">The path of directory to search.</param>
        /// <returns>Read-only dictionary with names of solvable problems as keys and corresponding Task Solvers as values.</returns>
        /// <exception cref="_15pl04.Ucc.Commons.Exceptions.TaskSolverLoadingException">Thrown when exception occured
        /// during loading task solvers.</exception>
        public static ReadOnlyDictionary<string, Type> GetTaskSolversFromPath(string taskSolversDirectoryPath)
        {
            var dictionary = new Dictionary<string, Type>();

            // add <key,value> pairs based on types derived from TaskSolver in *.dll files
            string[] fileNames;
            try
            {
                fileNames = Directory.GetFiles(taskSolversDirectoryPath, "*.dll");
            }
            catch (Exception ex)
            {
                throw new TaskSolverLoadingException("Couldn't get files from given directory.", ex);
            }
            var typeOfTaskSolver = typeof(TaskSolver);
            foreach (var fileName in fileNames)
            {
                var assembly = Assembly.LoadFile(fileName);
                var taskSolverTypes =
                    assembly.GetTypes().Where(t => typeOfTaskSolver.IsAssignableFrom(t) && !t.IsAbstract);
                foreach (var taskSolverType in taskSolverTypes)
                {
                    /* NOTE: there's a possibilty of throwing an Exception because TaskSolver has
                     * only one constructor which takes byte[] as parameter and behavior of it can
                     * vary on specific implementation */
                    TaskSolver taskSolver;
                    try
                    {
                        taskSolver = (TaskSolver)Activator.CreateInstance(taskSolverType, new byte[0]);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            taskSolver = (TaskSolver)Activator.CreateInstance(taskSolverType, null);
                        }
                        catch (Exception)
                        {
                            throw new TaskSolverLoadingException(
                                string.Format("Couldn't create instance of {0} to get problem name.", taskSolverType));
                        }
                    }
                    try
                    {
                        dictionary.Add(taskSolver.Name, taskSolverType);
                    }
                    catch (ArgumentNullException)
                    {
                        throw new TaskSolverLoadingException(
                            string.Format("Problem name for {0} task solver is null.", taskSolverType));
                    }
                    catch (ArgumentException)
                    {
                        throw new TaskSolverLoadingException(
                            string.Format("More then one task solver exists for problem: \"{0}\".", taskSolver.Name));
                    }
                }
            }

            var readOnlyDictionary = new ReadOnlyDictionary<string, Type>(dictionary);
            return readOnlyDictionary;
        }
    }
}