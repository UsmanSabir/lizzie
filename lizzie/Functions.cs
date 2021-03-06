﻿/*
 * Copyright (c) 2018 Thomas Hansen - thomas@gaiasoul.com
 *
 * Licensed under the terms of the MIT license, see the enclosed LICENSE
 * file for details.
 */

using System.Linq;
using System.Collections.Generic;
using lizzie.exceptions;

namespace lizzie
{
    /// <summary>
    /// Contains all the standard 'keyword' functions in Lizzie.
    /// </summary>
    public static class Functions<TContext>
    {
        /// <summary>
        /// Declares a symbol, optionally setting its initial value.
        /// 
        /// Expects the first argument to be a symbol, referenced by an '@' prefix character,
        /// and the optional second argument to be the initial value for that symbol.
        /// If no second argument is provided, the initial value of the symbol
        /// will be set to 'null'. This function will return the initial value of your symbol.
        /// </summary>
        /// <value>The function wrapping the 'var keyword'.</value>
        public static Function<TContext> Var => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking invocation.
            if (arguments.Count == 0)
                throw new LizzieRuntimeException("No arguments provided to 'var', provide at least a symbol name, e.g. 'var(@foo)'.");
            
            // Expecting symbol name as the first argument and doing some basic sanity checking.
            var symbolObject = arguments.Get(0);
            if (symbolObject == null)
                throw new LizzieRuntimeException("You'll need to supply a symbol name for 'var' to function correctly.");
            var symbolName = symbolObject as string;
            if (symbolName == null)
                throw new LizzieRuntimeException("You'll need to supply a symbol name as a string for 'var' to function correctly.");
            Compiler.SanityCheckSymbolName(symbolName);

            /*
             * Sanity checking to make sure symbol doesn't already exist.
             *
             * NOTICE!
             * We still allow the caller to override a statically bound keyword, but
             * not to declare the same variable twice in the same stack.
             */
            if (binder.ContainsKey(symbolName))
                throw new LizzieRuntimeException($"The symbol '{symbolName}' has already been declared in the scope of where you are trying to declare it using the 'var' keyword.");

            // More sanity checks.
            if (arguments.Count > 2)
                throw new LizzieRuntimeException($"The 'var' keyword can only handle at most two arguments, and you tried to pass in more than two arguments as you tried to declare '{symbolName}'.");

            /*
             * Retrieving the initial value, if any, for the symbol to push
             * onto the stack. If no variable is supplied, we default the
             * stack object's value to 'null'.
             */
            var value = arguments.Count > 1 ? arguments.Get(1) : null;

            // Setting the variable.
            binder[symbolName] = value;
            return value;
        });

        /// <summary>
        /// Allows you to change the value of a previously declared symbol.
        /// 
        /// Expects the first argument to be a symbol, prefixed by an '@'
        /// character, and the optional second argument to be the new value for that symbol.
        /// If no second argument is provided, the new value of the symbol
        /// will be set to 'null'.
        /// This function will return the new value of your symbol.
        /// </summary>
        /// <value>The function wrapping the 'set keyword'.</value>
        public static Function<TContext> Set => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking invocation.
            if (arguments.Count == 0)
                throw new LizzieRuntimeException("No arguments provided to 'set', provide at least a symbol name, e.g. 'set(@foo)'.");

            // Expecting symbol name as the first argument and doing some basic sanity checking.
            var symbolObject = arguments.Get(0);
            if (symbolObject == null)
                throw new LizzieRuntimeException("You'll need to supply a symbol name for 'set' to function correctly.");
            var symbolName = symbolObject as string;
            if (symbolName == null)
                throw new LizzieRuntimeException("You'll need to supply a symbol name as a string for 'set' to function correctly.");

            // Sanity checking that symbol already exists.
            if (!binder.ContainsKey(symbolName))
                throw new LizzieRuntimeException($"The symbol '{symbolName}' has not been declared in the scope of where you are trying to 'set' it.");

            // More sanity checks.
            if (arguments.Count > 2)
                throw new LizzieRuntimeException($"The 'set' keyword can only handle at most two arguments, and you tried to pass in more than two arguments as you tried to change the value of '{symbolName}'.");

            /*
             * Retrieving the initial value, if any, for the symbol to push
             * onto the stack. If no variable is supplied, we default the
             * stack object's value to 'null'.
             */
            var value = arguments.Count > 1 ? arguments.Get(1) : null;

            // Setting the variable.
            binder[symbolName] = value;
            return value;
        });

        /// <summary>
        /// Adds a bunch of things together. Can be used either for string literals,
        /// integer numbers, or floating point numbers - Or anything that has operator + overload in fact.
        /// 
        /// Will return the result of the addition to caller.
        /// </summary>
        /// <value>The function wrapping the 'add keyword'.</value>
        public static Function<TContext> Add => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking code.
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'add' keyword requires at least 2 arguments, and you tried to invoke it with fewer.");

            // Retrieving the first value, making sure we retrieve it as a "dynamic type".
            dynamic result = arguments.Get(0);
            foreach (dynamic ix in arguments.Skip(1)) {
                result += ix;
            }

            // Returning the result of the operation to caller.
            return result;
        });

        /// <summary>
        /// Subtracts a bunch of things from the first thing provided. Can be used
        /// for anything that has overloaded the - operator.
        /// 
        /// Will return the result of the subtraction to caller.
        /// </summary>
        /// <value>The function wrapping the 'subtract keyword'.</value>
        public static Function<TContext> Subtract => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking code.
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'subtract' keyword requires at least 2 arguments, and you tried to invoke it with fewer.");

            // Retrieving the first value, making sure we retrieve it as a "dynamic type".
            dynamic result = arguments.Get(0);
            foreach (dynamic ix in arguments.Skip(1)) {
                result -= ix;
            }

            // Returning the result of the operation to caller.
            return result;
        });

        /// <summary>
        /// Multiplies a bunch of things with each other. Can be used for anything that
        /// has overloaded the * operator.
        /// 
        /// Will return the result of the multiplication to caller.
        /// </summary>
        /// <value>The function wrapping the 'multiply keyword'.</value>
        public static Function<TContext> Multiply => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking code.
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'multiply' keyword requires at least 2 arguments, and you tried to invoke it with fewer.");

            // Retrieving the first value, making sure we retrieve it as a "dynamic type".
            dynamic result = arguments.Get(0);
            foreach (dynamic ix in arguments.Skip(1)) {
                result *= ix;
            }

            // Returning the result of the operation to caller.
            return result;
        });

        /// <summary>
        /// Divides a bunch of things from the first thing. Can be used by anything
        /// that has overloaded the / operator.
        /// 
        /// Will return the result of the division to caller.
        /// </summary>
        /// <value>The function wrapping the 'divide keyword'.</value>
        public static Function<TContext> Divide => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking code.
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'divide' keyword requires at least 2 arguments, and you tried to invoke it with fewer.");

            // Retrieving the first value, making sure we retrieve it as a "dynamic type".
            dynamic result = arguments.Get(0);
            foreach (dynamic ix in arguments.Skip(1)) {
                result /= ix;
            }

            // Returning the result of the operation to caller.
            return result;
        });

        /// <summary>
        /// Calculates the modulo of two or more things. Can be used for anything
        /// that has overloaded the % operator.
        /// 
        /// Will return the result to caller.
        /// </summary>
        /// <value>The function wrapping the 'modulo keyword'.</value>
        public static Function<TContext> Modulo => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking code.
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'modulo' keyword requires at least 2 arguments, and you tried to invoke it with fewer.");

            // Retrieving the first value, making sure we retrieve it as a "dynamic type".
            dynamic result = arguments.Get(0);
            foreach (dynamic ix in arguments.Skip(1)) {
                result %= ix;
            }

            // Returning the result of the operation to caller.
            return result;
        });

        /// <summary>
        /// Creates a new function and returns it to the caller.
        /// 
        /// The first argument is expected to be a lambda block, e.g. "{ ... your code goes here ... }".
        /// Optionally supply one or more symbols as e.g. "@foo" as additional
        /// arguments, to allow for referencing arguments passed into the function
        /// inside of the function by name.
        /// </summary>
        /// <value>The function wrapping the 'function keyword'.</value>
        public static Function<TContext> Function => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking code.
            if (arguments.Count == 0)
                throw new LizzieRuntimeException("The 'function' keyword requires at least 1 argument, and you tried to invoke it with fewer.");

            // Retrieving lambda of function and doing some basic sanity checks.
            var lambda = arguments.Get(0) as Function<TContext>;
            if (lambda == null)
                throw new LizzieRuntimeException("When declaring a function, the first argument must be a lambda object, e.g. '{ ... some code ... }'.");

            // Retrieving argument declarations and sanity checking them.
            var args = arguments.Skip(1).Select(ix => ix as string).ToList();
            foreach (var ix in args) {
                if (ix is string ixStr)
                    Compiler.SanityCheckSymbolName(ixStr);
                else
                    throw new LizzieRuntimeException("When declaring a function, all argument declarations must be valid symbols, e.g. '@foo'.");
            }

            /*
             * Returning function to caller.
             *
             * NOTICE!
             * A function always pushes the stack.
             */
            return new Function<TContext>((ctx2, binder2, arguments2) => {

                /*
                 * Sanity checking that caller did not supply more arguments than
                 * the function is declared to at maximum being able to handle.
                 */
                if (arguments2.Count > args.Count)
                    throw new LizzieRuntimeException("Function was invoked with too many arguments.");

                // Pushing stack, making sure we can correctly pop it once we're done.
                binder2.PushStack();
                try {

                    /*
                     * Binding all argument declarations for our lambda to whatever
                     * the caller provided as values during invocation.
                     */
                    for (var ix = 0; ix < arguments2.Count && ix < args.Count; ix++) {
                        binder2[args[ix]] = arguments2.Get(ix);
                    }

                    /*
                     * Applying the rest of our arguments as null values.
                     *
                     * NOTICE!
                     * This allows us to create functions without forcing the caller
                     * of those functions to supply all arguments that the function
                     * declares, simply setting the rest of the declared arguments
                     * to "null" values.
                     */
                    for (var ix = arguments2.Count; ix < args.Count; ix++) {
                        binder2[args[ix]] = null;
                    }

                    // Evaluating function.
                    return lambda(ctx2, binder2, arguments2);

                } finally {

                    // Popping stack.
                    binder.PopStack();
                }
            });
        });

        /// <summary>
        /// Creates an if condition and returns it to the caller.
        ///
        /// The first argument is expected to be some condition, the second argument
        /// is expected to be a lambda which will be evaluated if the condition
        /// evaluates to anything but null, and the third (optional) argument is
        /// what should be evaluated if the condition evaluates to null.
        /// When 'if' is evaluated, it will return either null, or the returned
        /// value from the lambda object that was evaluated as a response to the
        /// condition's value.
        /// </summary>
        /// <value>The function wrapping the 'if keyword'.</value>
        public static Function<TContext> If => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking code.
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'if' keyword requires at least 2 arguments, and you tried to invoke it with fewer.");

            // Retrieving condition, if(true) lambda, and sanity checking invocation.
            var condition = arguments.Get(0);
            var lambdaIf = arguments.Get(1) as Function<TContext>;
            if (lambdaIf == null)
                throw new LizzieRuntimeException("The 'if' keyword requires a lambda argument as its second argument.");

            // Retrieving an sanity checking if(false) lambda, if it exists.
            var arg3 = arguments.Count > 2 ? arguments.Get(2) : null;
            Function<TContext> lambdaElse = null;
            if (arg3 != null) {
                lambdaElse = arg3 as Function<TContext>;
                if (lambdaElse == null)
                    throw new LizzieRuntimeException("The 'if' keyword requires a lambda argument as its third (else) argument if you supply a third argument.");
            }

            // Checking if we should evaluate if(true) lambda.
            if (condition != null) {

                // If yields true, evaluating if(true) lambda.
                return lambdaIf(ctx, binder, arguments);

            } else if (lambdaElse != null) {

                // If yields false, and we have an else lambda we should evaluate.
                return lambdaElse(ctx, binder, arguments);
            }

            // If yields false, and there is no "else lambda".
            return null;
        });

        /// <summary>
        /// Creates an equals function, that checks two or more objects for equality.
        ///
        /// This function will return null if any of the objects it is being asked
        /// to compare does not equal all other objects it is asked to compare.
        /// </summary>
        /// <value>The function wrapping the 'eq keyword'.</value>
        public static Function<TContext> Eq => new Function<TContext>((ctx, binder, arguments) =>
        {
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'eq' function must be given at least 2 arguments.");
            var arg1 = arguments.Get(0);

            // Comparing all other objects to the first.
            foreach (var ix in arguments.Skip(1)) {
                if (arg1 == null && ix != null || arg1 != null && ix == null)
                    return null;
                if (arg1 != null && !arg1.Equals(ix))
                    return null;
            }

            // Equality!
            return (object)true;
        });

        /// <summary>
        /// Creates a more than function, that checks if one object is more than the other.
        ///
        /// This function will return the LHS object if it is more than the RHS object,
        /// otherwise it will return the null. This function can be used for any object
        /// that has overloaded the more than operator.
        /// </summary>
        /// <value>The function wrapping the 'mt keyword'.</value>
        public static Function<TContext> Mt => new Function<TContext>((ctx, binder, arguments) =>
        {
            if (arguments.Count != 2)
                throw new LizzieRuntimeException("The 'more than' function must be given exactly 2 arguments.");
            dynamic arg1 = arguments.Get(0);
            dynamic arg2 = arguments.Get(1);
            if (arg1 > arg2)
                return arg1;

            // Failed!
            return null;
        });

        /// <summary>
        /// Creates a less than function, that checks if one object is less than the other.
        ///
        /// This function will return the LHS object if it is less than the RHS object,
        /// otherwise it will return the null. This function can be used for any object
        /// that has overloaded the less than operator.
        /// </summary>
        /// <value>The function wrapping the 'lt keyword'.</value>
        public static Function<TContext> Lt => new Function<TContext>((ctx, binder, arguments) =>
        {
            if (arguments.Count != 2)
                throw new LizzieRuntimeException("The 'less than' function must be given exactly 2 arguments.");
            dynamic arg1 = arguments.Get(0);
            dynamic arg2 = arguments.Get(1);
            if (arg1 < arg2)
                return arg1;

            // Failed!
            return null;
        });

        /// <summary>
        /// Creates a more than equals function, that checks if one object is more
        /// than or equals to the other.
        ///
        /// This function will return the LHS object if it is more than or equals the RHS object,
        /// otherwise it will return the null. This function can be used for any object
        /// that has overloaded the more than equals operator.
        /// </summary>
        /// <value>The function wrapping the 'mte keyword'.</value>
        public static Function<TContext> Mte => new Function<TContext>((ctx, binder, arguments) =>
        {
            if (arguments.Count != 2)
                throw new LizzieRuntimeException("The 'more than equals' function must be given exactly 2 arguments.");
            dynamic arg1 = arguments.Get(0);
            dynamic arg2 = arguments.Get(1);
            if (arg1 >= arg2)
                return arg1;

            // Failed!
            return null;
        });

        /// <summary>
        /// Creates a less than equals function, that checks if one object is less or
        /// equals the other.
        ///
        /// This function will return the LHS object if it is less than or equals to the
        /// RHS object, otherwise it will return the null. This function can be used
        /// for any object that has overloaded the less than equals operator.
        /// </summary>
        /// <value>The function wrapping the 'lte keyword'.</value>
        public static Function<TContext> Lte => new Function<TContext>((ctx, binder, arguments) =>
        {
            if (arguments.Count != 2)
                throw new LizzieRuntimeException("The 'less than' function must be given exactly 2 arguments.");
            dynamic arg1 = arguments.Get(0);
            dynamic arg2 = arguments.Get(1);
            if (arg1 <= arg2)
                return arg1;

            // Failed!
            return null;
        });

        /// <summary>
        /// Negates the given value, whatever it previously was.
        ///
        /// If the argument given to this function is null, this function will
        /// return true. If it is given a non-null value, the function will return
        /// null. This negates its argument, allowing you to do the same as the
        /// not operator normally would do in a conventional programming language.
        /// </summary>
        /// <value>The function wrapping the 'not keyword'.</value>
        public static Function<TContext> Not => new Function<TContext>((ctx, binder, arguments) =>
        {
            if (arguments.Count != 1)
                throw new LizzieRuntimeException("The 'not' function must be given exactly 1 argument.");
            var arg = arguments.Get(0);
            return arg == null ? (object)true : null;
        });

        /// <summary>
        /// Returns the first object it is being given that is not null.
        /// 
        /// If all objects are null, this function will return null.
        /// </summary>
        /// <value>The function wrapping the 'any keyword'.</value>
        public static Function<TContext> Any => new Function<TContext>((ctx, binder, arguments) =>
        {
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'any' function must be given at least 2 arguments.");
            return arguments.FirstOrDefault(ix => {

                // Making sure we verify that this is a "delayed verification" of condition.
                if (ix is Function<TContext> function) {

                    // Checking if function returns something, and returning predicate value accordingly.
                    var ixContent = function(ctx, binder, arguments);
                    if (ixContent == null)
                        return false;
                    return true;

                } else if (ix is string symbol) {

                    // Symbol.
                    var symbolValue = binder[symbol];
                    if (symbolValue == null)
                        return false;
                    return true;

                } else {

                    throw new LizzieRuntimeException("The 'any' function requires you to only pass in conditions as function invocations. Prepend your condition with an '@' sign if this is a problem.");
                }
            });
        });

        /// <summary>
        /// Returns true if all of its given values yields true.
        /// 
        /// This function will only return true if all of its arguments are not null.
        /// Otherwise the function will return null.
        /// </summary>
        /// <value>The function wrapping the 'all keyword'.</value>
        public static Function<TContext> All => new Function<TContext>((ctx, binder, arguments) =>
        {
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'all' function must be given at least 2 arguments.");
            foreach (var arg in arguments) {
                if (arg == null) {

                    throw new LizzieRuntimeException("The 'all' function requires you to only pass in function invocations. Prepend your condition with an '@' sign if this is a problem.");

                } else {

                    /*
                     * Checking if this is a function, at which point we evaluate it,
                     * to make sure we support "delayed function invocations".
                     */
                    if (arg is Function<TContext> functor) {

                        // Making sure we verify that this is a "delayed verification" of condition.
                        if (functor(ctx, binder, arguments) == null)
                            return null; // No reasons to continue ...

                    } else if (arg is string symbol) {

                        // Symbol.
                        var symbolValue = binder[symbol];
                        if (symbolValue == null)
                            return null;

                    } else {

                        throw new LizzieRuntimeException("The 'all' function requires you to only pass in function invocations. Prepend your condition with an '@' sign if this is a problem.");
                    }
                }
            }
            return arguments.Last();
        });

        /// <summary>
        /// Creates a list of objects and returns it to the caller.
        ///
        /// This function will create a list, with the initial arguments as its
        /// members, and return that list to the caller.
        /// </summary>
        /// <value>The function wrapping the 'list keyword'.</value>
        public static Function<TContext> List => new Function<TContext>((ctx, binder, arguments) =>
        {
            return new List<object>(arguments);
        });

        /// <summary>
        /// Returns the count of a list previously created.
        ///
        /// This function will return the count of items in the list it is given
        /// as its first argument.
        /// </summary>
        /// <value>The function wrapping the 'count keyword'.</value>
        public static Function<TContext> Count => new Function<TContext>((ctx, binder, arguments) =>
        {
            if (arguments.Count != 1)
                throw new LizzieRuntimeException("The 'count' function must be given exactly 1 argument, and that argument must be a list.");
            var list = arguments.Get(0) as List<object>;
            if (list == null)
                throw new LizzieRuntimeException("The 'count' function must be given a list as its only argument.");
            return list.Count;
        });

        /// <summary>
        /// Returns the specified item of a list previously created.
        ///
        /// This function will return the object at the specified index from a previously
        /// created list. Pass in the index of the item to retrieve as the only
        /// argument you supply to this function.
        /// </summary>
        /// <value>The function wrapping the 'get keyword'.</value>
        public static Function<TContext> Get => new Function<TContext>((ctx, binder, arguments) =>
        {
            if (arguments.Count != 2)
                throw new LizzieRuntimeException("The 'get' function must be given exactly 2 arguments. The first argument must be a list, and the second argument a numeric value.");
            var list = arguments.Get(0) as List<object>;
            if (list == null)
                throw new LizzieRuntimeException("The 'get' function must be given a list as its first argument.");
            var index = arguments.Get<int>(1);
            if (index >= list.Count)
                throw new LizzieRuntimeException($"There are not '{index}' items in your list.");
            return list[index];
        });

        /// <summary>
        /// Adds the specified items to the specified list.
        ///
        /// This function will add all arguments beyond the first to the list
        /// specified as the first argument. The function will return the last
        /// items added to your list.
        /// </summary>
        /// <value>The function wrapping the 'add keyword'.</value>
        public static Function<TContext> AddList => new Function<TContext>((ctx, binder, arguments) =>
        {
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'add' function must be given at least 2 arguments. The first argument must be a list, and the rest of the arguments objects to add to your list.");
            var list = arguments.Get(0) as List<object>;
            if (list == null)
                throw new LizzieRuntimeException("The 'add' function must be given a list as its first argument.");
            list.AddRange(arguments.Skip(1));
            return list[list.Count -1];
        });

        /// <summary>
        /// Returns a slice from the specified list without modifying the original list.
        ///
        /// This function will return a 'slice' of the list supplied as its first argument.
        /// It expects at least 1 argument, and that argument must be a list. If it
        /// is given a second argument, this argument is expected to be a numeric value,
        /// defining the offset of where to start creating the slice. If it is given
        /// a third argument, it expects that argument to be the end of where to
        /// create the slice from.
        /// </summary>
        /// <value>The function wrapping the 'slice keyword'.</value>
        public static Function<TContext> Slice => new Function<TContext>((ctx, binder, arguments) =>
        {
            if (arguments.Count == 0 || arguments.Count > 3)
                throw new LizzieRuntimeException("The 'slice' function must be given 1-3 arguments, the first argument must be a list, and the optional 2nd and 3rd arguments must be numeric values.");
            var list = arguments.Get(0) as List<object>;
            if (list == null)
                throw new LizzieRuntimeException("The 'slice' function must be given a list as its first argument.");
            var start = 0;
            var end = list.Count;
            if (arguments.Count > 1)
                start = arguments.Get<int>(1);
            if (arguments.Count > 2)
                end = arguments.Get<int>(2);
            if (end <= start)
                throw new LizzieRuntimeException("The end index to 'slice' must be larger than the start index.");
            return list.GetRange(start, end - start);
        });

        /// <summary>
        /// Iterates a list and evaluates the specified lambda once for each of its values.
        /// 
        /// The first argument must be a symbol, declaring how you
        /// intend to reference your iterated list item from within your lambda block.
        /// The second argument must be a list.
        /// The third argument is expected to be a lambda block, e.g. "{ ... your code goes here ... }".
        /// The function will return the list itself.
        /// </summary>
        /// <value>The function wrapping the 'each keyword'.</value>
        public static Function<TContext> Each => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking code.
            if (arguments.Count < 3)
                throw new LizzieRuntimeException("The 'each' keyword requires 3 arguments, and you tried to invoke it with fewer.");

            // Retrieving lambda and list for function and doing some basic sanity checks.
            var argName = arguments.Get(0) as string;
            if (argName == null)
                throw new LizzieRuntimeException("The 'each' function must be given a symbol name as its first argument.");
            var list = arguments.Get(1) as List<object>;
            if (list == null)
                throw new LizzieRuntimeException("The 'each' function must be given a list as its second argument.");
            var lambda = arguments.Get(2) as Function<TContext>;
            if (lambda == null)
                throw new LizzieRuntimeException("When invoking the 'each' function, the third argument must be a lambda object, e.g. '{ ... some code ... }'.");

            // Making sure our stack does not contain the same variable from before.
            if (binder.ContainsDynamicKey(argName))
                throw new LizzieRuntimeException($"The '{argName}' is already declared from before, hence you can't use it as an iterator for the 'each' function.");

            /*
             * Looping through each item in list, evaluating lambda once for each item, making
             * sure we can return a new list containing all items where our lambda yielded a non-null value.
             */
            try {
                foreach (var ix in list) {
                    binder[argName] = ix;
                    var current = lambda(ctx, binder, arguments);
                }
            } finally {
                if (binder.ContainsDynamicKey(argName))
                    binder.RemoveKey(argName);
            }
            return list;
        });

        /// <summary>
        /// Converts the specified object to a string.
        ///
        /// This function will return the string representation of whatever object
        /// it is given. The function must be given exactly one argument.
        /// </summary>
        /// <value>The function wrapping the 'string keyword'.</value>
        public static Function<TContext> String => new Function<TContext>((ctx, binder, arguments) =>
        {
            if (arguments.Count != 1)
                throw new LizzieRuntimeException("The 'string' function must be given exactly 1 argument.");
            return arguments.Get<string>(0);
        });

        /// <summary>
        /// Converts the specified object to a number.
        ///
        /// This function will return the numeric representation of whatever object
        /// it is given. The function must be given exactly one argument.
        /// </summary>
        /// <value>The function wrapping the 'number keyword'.</value>
        public static Function<TContext> Number => new Function<TContext>((ctx, binder, arguments) =>
        {
            if (arguments.Count != 1)
                throw new LizzieRuntimeException("The 'number' function must be given exactly 1 argument.");
            var tmp = arguments.Get(0);
            if (tmp is long tmpLong)
                return tmpLong;
            if (tmp is double tmpDouble)
                return tmpDouble;
            var tmpString = arguments.Get<string>(0);
            if (tmpString.Contains("."))
                return double.Parse(tmpString);
            return long.Parse(tmpString);
        });

        /// <summary>
        /// Returns a substring of the given string value.
        ///
        /// This function will return a substring of the specified string. It
        /// expects at least two arguments, the first being a string, the second
        /// an offset of where to start the returned string from. You can also
        /// optionally supply a third argument
        /// </summary>
        /// <value>The function wrapping the 'substr keyword'.</value>
        public static Function<TContext> Substr => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking.
            if (arguments.Count < 2)
                throw new LizzieRuntimeException("The 'substr' function must be given at least 2 arguments.");

            // Retrieving string to manipulate.
            var arg1 = arguments.Get<string>(0);

            // Retrieving start position.
            var arg2 = arguments.Get<int>(1);

            // Checking if we have an end position.
            if (arguments.Count > 2) {
                return arg1.Substring(arg2, arguments.Get<int>(2));
            }
            return arg1.Substring(arg2);
        });

        /// <summary>
        /// Returns the length of the given string value.
        ///
        /// This function will return the length of the given string value.
        /// </summary>
        /// <value>The function wrapping the 'length keyword'.</value>
        public static Function<TContext> Length => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking.
            if (arguments.Count != 1)
                throw new LizzieRuntimeException("The 'length' function must be given exactly 1 argument.");

            // Retrieving string's length.
            var arg1 = arguments.Get<string>(0);
            return arg1.Length;
        });

        /// <summary>
        /// Replace occurrencies of one string in a string with another value and
        /// returns a new string with the replacing having occurred.
        ///
        /// This function will replace all occurrencies of the 2nd argument with the
        /// value of the 3rd argument, and return a new string to the caller.
        /// </summary>
        /// <value>The function wrapping the 'replace keyword'.</value>
        public static Function<TContext> Replace => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking.
            if (arguments.Count != 3)
                throw new LizzieRuntimeException("The 'replace' function must be given exactly 3 arguments.");

            // Retrieving string's length.
            var arg1 = arguments.Get<string>(0);
            var arg2 = arguments.Get<string>(1);
            var arg3 = arguments.Get<string>(2);
            return arg1.Replace(arg2, arg3);
        });

        /// <summary>
        /// Dynamically compiles and evaluates the given code, and returns the result.
        ///
        /// This function will compile and evaluate the given string, assuming it
        /// contains valid Lizzie code. Notice, the evaluated code will not have
        /// access to the stack of the code evaluating eval, but it will share
        /// the same context instance.
        /// </summary>
        /// <value>The function wrapping the 'eval keyword'.</value>
        public static Function<TContext> Eval => new Function<TContext>((ctx, binder, arguments) =>
        {
            // Sanity checking.
            if (arguments.Count != 1)
                throw new LizzieRuntimeException("The 'eval' function must be given exactly 1 argument.");

            // Retrieving string's length.
            var arg1 = arguments.Get<string>(0);
            var lambda = LambdaCompiler.Compile<TContext>(ctx, arg1);
            return lambda();
        });
    }
}
