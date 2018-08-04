﻿/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Linq;
using System.Collections.Generic;
using poetic.lambda.utilities;

namespace poetic.lambda.collections
{
    /// <summary>
    /// Class encapsulating a list of Action delegates taking no arguments.
    /// </summary>
    public class Actions : Sequence<Action>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.collections.Actions"/> class.
        /// </summary>
        public Actions()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.collections.Actions"/> class.
        /// </summary>
        /// <param name="actions">Actions.</param>
        public Actions(params Action[] actions)
            : base(actions)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:poetic.lambda.collections.Actions"/> class.
        /// </summary>
        /// <param name="actions">Actions.</param>
        public Actions(IEnumerable<Action> actions)
            : base(actions)
        { }

        /// <summary>
        /// Sequentially executes each action not returning before execution is finished.
        /// </summary>
        public void ExecuteSequentiallyBlocked()
        {
            Executor.ExecuteSequentiallyBlocked(this);
        }

        /// <summary>
        /// Sequentially executes each action on a different thread, and blocking
        /// the calling thread until done, or millisecondsTimeout have passed.
        /// </summary>
        /// <param name="millisecondsTimeout">Maximum amount of time to block calling thread.</param>
        public void ExecuteSequentiallyBlocked(int millisecondsTimeout)
        {
            Executor.ExecuteSequentiallyBlocked(this, millisecondsTimeout);
        }

        /// <summary>
        /// Sequentially executes each action on a different thread without blocking
        /// the calling thread.
        /// </summary>
        public void ExecuteSequentiallyUnblocked()
        {
            Executor.ExecuteSequentiallyUnblocked(this);
        }

        /// <summary>
        /// Executes each action in parallel blocking the calling thread until
        /// all actions are finished executing.
        /// </summary>
        public void ExecuteParallelBlocked()
        {
            Executor.ExecuteParallelBlocked(this);
        }

        /// <summary>
        /// Executes each action in parallel blocking the calling thread for a
        /// maximum amount of time, until execution of all actions are finished,
        /// or milliseconds have passed, whatever occurs first.
        /// </summary>
        /// <param name="millisecondsTimeout">Maximum amount of time to block calling thread.</param>
        public void ExecuteParallelBlocked(int millisecondsTimeout)
        {
            Executor.ExecuteParallelBlocked(this, millisecondsTimeout);
        }

        /// <summary>
        /// Executes each action in parallel without blocking the calling thread.
        /// </summary>
        public void ExecuteParallelUnblocked()
        {
            Executor.ExecuteParallelUnblocked(this);
        }
    }
}