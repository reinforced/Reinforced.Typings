﻿using System;
using System.Collections.Generic;

namespace Reinforced.Typings.Ast.Dependency
{
    /// <summary>
    /// Import declaration
    /// </summary>
    public class RtImport : RtNode
    {
        private string _target;

        /// <summary>
        /// Targets list
        /// </summary>
        public string Target
        {
            get => _target;
            set
            {
                _target = value?.Trim();
                CheckWildcardImport();
            }
        }

        /// <summary>
        /// Gets flag whether RtImport is wildcard import
        /// </summary>
        public bool IsWildcard => WildcardAlias != null;

        /// <summary>
        /// Use double quotes instead of single quotes
        /// </summary>
        public bool UseDoubleQuotes { get; set; }

        /// <summary>
        /// Gets wildcard alias of import
        /// </summary>
        public string WildcardAlias { get; private set; }

        private void CheckWildcardImport()
        {
            if (_target.StartsWith("*"))
            {
                var arr = _target.Split(new[] { " as " }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length < 2)
                {
                    WildcardAlias = null;
                }
                else
                {
                    WildcardAlias = arr[1];
                }
            }
            else
            {
                WildcardAlias = null;
            }
        }

        /// <summary>
        /// Import source
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// When true, "from" part will be replaced with "= require('From')"
        /// </summary>
        public bool IsRequire { get; set; }

        /// <inheritdoc />
        public override IEnumerable<RtNode> Children
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public override void Accept(IRtVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override void Accept<T>(IRtVisitor<T> visitor)
        {
            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var quote = UseDoubleQuotes ? "\"" : "'";

            return IsRequire
                ? $"import {Target} = require({quote}{From}{quote});"
                : $"import {Target} from {quote}{From}{quote};";
        }
    }
}
