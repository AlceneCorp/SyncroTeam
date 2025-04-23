using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncroTeam.Domain.Entities
{
    /// <summary>
    /// Définit les règles d'affectation pour une activité :
    /// - Jours autorisés
    /// - Agents habilités à la réaliser
    /// </summary>
    public class ActivitySetting
    {
        /// <summary>
        /// Liste des jours où l'activité est autorisée
        /// </summary>
        public List<DayOfWeek> AllowedDays { get; set; } = new();

        /// <summary>
        /// Liste des noms d'agents autorisés à effectuer cette activité
        /// </summary>
        public List<String> AuthorizedAgents { get; set; } = new();

        public ActivitySetting() { }

        public ActivitySetting(IEnumerable<DayOfWeek> allowedDays, IEnumerable<String> authorizedAgents)
        {
            AllowedDays = new List<DayOfWeek>(allowedDays);
            AuthorizedAgents = new List<String>(authorizedAgents);
        }

        /// <summary>
        /// Vérifie si un agent est autorisé à effectuer cette activité
        /// </summary>
        public bool IsAgentAuthorized(string agentName)
        {
            return AuthorizedAgents.Any(agent => agent.Equals(agentName, StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        /// Vérifie si une activité peut être effectuée à un jour donné
        /// </summary>
        public bool IsDayAllowed(DayOfWeek day)
        {
            return AllowedDays.Contains(day);
        }
    }
}
