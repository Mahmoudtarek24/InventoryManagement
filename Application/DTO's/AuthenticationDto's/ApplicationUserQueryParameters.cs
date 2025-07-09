using Application.Constants.Enum;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web.Http.Description;
namespace Application.DTO_s.AuthenticationDto_s
{
	public class ApplicationUserQueryParameters :BaseQueryParameters
	{
		public UserOrdering UserOrdering { get; set; }
	}
}
