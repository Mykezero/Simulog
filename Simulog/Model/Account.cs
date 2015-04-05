namespace Simulog.Model
{
    /// <summary>
    /// Holds information necessary to login 
    /// a player's account. 
    /// </summary>
    public class Account
    {
        /// <summary>
        /// The account's name. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The account's password. 
        /// </summary>
        public string Password { get; set; }
    }
}
