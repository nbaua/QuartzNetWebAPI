﻿using System;

namespace QuartzNetWebAPI.Models.Jobs
{
    /// <summary>
    /// Data model for displaying job details in a list.
    /// </summary>
    public sealed class JobListDetail
    {
        /// <summary>
        /// DisallowConcurrentExecution attribute, this applies to a job definition instance, not a job class instance,
        /// though it was decided to have the job class carry the attribute because it does often make a difference to how the class is coded
        /// (e.g. the ‘statefullness’ will need to be explicitly ‘understood’ by the code within the execute method).
        /// </summary>
        public bool ConcurrentExecutionDissallowed { get; set; }
        /// <summary>
        /// If a job is non-durable, it is automatically deleted from the scheduler once there are no longer any active triggers associated with it.
        /// In other words, non-durable jobs have a life span bounded by the existence of its triggers.
        /// </summary>
        public bool Durable { get; set; }
        /// <summary>
        /// This is a attribute that can be added to the Job class that tells Quartz to update the stored copy of the
        /// JobDetail’s JobDataMap after the Execute() method completes successfully (without throwing an exception),
        /// such that the next execution of the same job (JobDetail) receives the updated values rather than the originally stored values.
        /// </summary>
        public bool PersistJobDataAfterExecution { get; set; }
        /// <summary>
        /// If a job “requests recovery”, and it is executing during the time of a ‘hard shutdown’ of the scheduler
        /// (i.e. the process it is running within crashes, or the machine is shut off), then it is re-executed when the scheduler is started again.
        /// In this case, the JobExecutionContext.Recovering property will return true.
        /// </summary>
        public bool RequestRecovery { get; set; }
        /// <summary>
        /// The name of the job.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The name of the Group where the job is configured.
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// The description of the job.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The fully qualified name of the job.
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// Date stamp when job was executed the last time.
        /// </summary>
        public DateTimeOffset? LastFireTimeUtc { get; set; }

        /// <summary>
        /// Date stamp indicating the next time the job will be executed.
        /// </summary>
        public DateTimeOffset? NextFireTimeUtc { get; set; }
    }
}
