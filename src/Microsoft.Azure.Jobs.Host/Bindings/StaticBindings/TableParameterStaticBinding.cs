﻿using System.Reflection;
using Microsoft.Azure.Jobs.Host.Protocols;

namespace Microsoft.Azure.Jobs
{
    internal class TableParameterStaticBinding : ParameterStaticBinding
    {
        private string _tableName;

        public string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                TableClient.ValidateAzureTableName(value);
                _tableName = value;
            }
        }

        public override void Validate(IConfiguration config, ParameterInfo parameter)
        {
            // Table name was already validated in property-setter

            var type = parameter.ParameterType;
            TableParameterRuntimeBinding.GetTableBinderOrThrow(config, type, false);
        }

        public override ParameterRuntimeBinding Bind(IRuntimeBindingInputs inputs)
        {
            return new TableParameterRuntimeBinding
            {
                Name = Name,
                Table = new CloudTableDescriptor
                {
                    AccountConnectionString = inputs.AccountConnectionString,
                    TableName = this.TableName
                }
            };
        }

        public override ParameterRuntimeBinding BindFromInvokeString(IRuntimeBindingInputs inputs, string invokeString)
        {
            // For convenience, do the right thing with an empty string
            if (string.IsNullOrWhiteSpace(invokeString))
            {
                invokeString = this.TableName;
            }

            return new TableParameterRuntimeBinding
            {
                Name = Name,
                Table = new CloudTableDescriptor
                {
                    AccountConnectionString = inputs.AccountConnectionString,
                    TableName = invokeString
                }
            };
        }

        public override string Description
        {
            get
            {
                return string.Format("Access table: {0}", this.TableName);
            }
        }

        public override string Prompt
        {
            get
            {
                return "Enter the table name";
            }
        }

        public override string DefaultValue
        {
            get
            {
                return TableName;
            }
        }

        public override ParameterDescriptor ToParameterDescriptor()
        {
            return new TableParameterDescriptor
            {
                TableName = TableName
            };
        }
    }
}
