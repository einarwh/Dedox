using System.Collections.Generic;

namespace Dedox
{
    public interface IFoo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="that">
        /// The that.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int this[int that] { get; set; }

        /// <summary>
        /// Gets or sets the things.
        /// </summary>
        /// <value>
        /// The things.
        /// </value>
        List<Queue<int>> Things { get; set; }

        /// <summary>
        /// Gets the things.
        /// </summary>
        /// <returns></returns>
        List<Queue<int>> GetThings();
    }
}
