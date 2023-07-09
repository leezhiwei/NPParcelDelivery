using Microsoft.AspNetCore.Mvc;
using NPParcelDeliveryServiceAssignment.DALs;
using System.ComponentModel.DataAnnotations;

namespace NPParcelDeliveryServiceAssignment.Models
{
	public class RegisterValidation:ValidationAttribute
	{
		private MemberDAL mdal = new MemberDAL();
		public override bool IsValid(object Value)
		{
			string email = Value as string;
			List<Member> mlist = mdal.GetAllMember();
			foreach (Member m in mlist)
			{
				if (m.EmailAddr == email)
				{
					return false;
				}
			}
			return true;
		}
	}
}
