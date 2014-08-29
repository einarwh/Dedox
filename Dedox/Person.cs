using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dedox
{
    public class Person
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// The find address by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="Address"/>.
        /// </returns>
        public Address FindAddressByIdAndDate(int id, DateTime dateTime)
        {
            return new Address();
        }
    }

    public class Address
    {
        public string Street { get; set; }

        public int Number { get; set; }
    }
}
