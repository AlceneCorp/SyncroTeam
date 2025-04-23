using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncroTeam.Domain.Settings
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
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
        public List<string> AuthorizedAgents { get; set; } = new();

        
        public ActivitySetting() { }

        public ActivitySetting(IEnumerable<DayOfWeek> allowedDays, IEnumerable<string> authorizedAgents)
        {
            AllowedDays = new List<DayOfWeek>(allowedDays);
            AuthorizedAgents = (authorizedAgents == null) ? null : new List<String>(authorizedAgents);
        }

        /// <summary>
        /// Vérifie si un agent est autorisé à effectuer cette activité
        /// </summary>
        public bool IsAgentAuthorized(string agentName)
        {
            // Si null => tous les agents sont autorisés
            if(AuthorizedAgents == null) return true;
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
