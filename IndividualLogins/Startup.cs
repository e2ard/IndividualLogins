using IndividualLogins.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IndividualLogins.Startup))]
namespace IndividualLogins
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRoles();
            CreateUsers();
        }

        // In this method we will create default User roles and Admin user for login   
        private void CreateRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Admin"))
            {
            // first we create Admin rool   
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("Edit"))
            {
                var role = new IdentityRole();
                role.Name = "Edit";
                roleManager.Create(role);
            }

            // creating Creating Employee role    
            if (!roleManager.RoleExists("Preview"))
            {
                var role = new IdentityRole();
                role.Name = "Preview";
                roleManager.Create(role);
            }
        }

        private void CreateUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var user = new ApplicationUser();
            user.UserName = "e2ard";
            user.Email = "edvard.naus@gmail.com";
            string userPWD = "421c3421c3";
            var chkUser = UserManager.Create(user, userPWD);

            //Add default User to Role Admin   
            if (chkUser.Succeeded)
            {
                UserManager.AddToRole(user.Id, "Admin");
            }

            var userEdit = new ApplicationUser();
            userEdit.UserName = "edit";
            userEdit.Email = "edvard.naus@gmail.com";
            string userEditPWD = "421c3421c3";
            chkUser = UserManager.Create(userEdit, userEditPWD);
            //Add default User to Role Edit   
            if (chkUser.Succeeded)
            {
                var result1 = UserManager.AddToRole(userEdit.Id, "Edit");
            }

            var userPreview = new ApplicationUser();
            userPreview.UserName = "preview";
            userPreview.Email = "edvard.naus@gmail.com";
            string userPreviewPWD = "421c3421c3";
            chkUser = UserManager.Create(userPreview, userPreviewPWD);

            //Add default User to Role Preview   
            if (chkUser.Succeeded)
            {
                var result1 = UserManager.AddToRole(userPreview.Id, "Preview");
            }

            var userInit = new ApplicationUser();
            userInit.UserName = "sales";
            userInit.Email = "sales@pricingtool.eu";
            string userInitPwd = "Sales123.";
            chkUser = UserManager.Create(userInit, userInitPwd);

            //Add default User to Role Preview   
            if (chkUser.Succeeded)
            {
                var result1 = UserManager.AddToRole(userInit.Id, "Preview");
            }

            userInit = new ApplicationUser();
            userInit.UserName = "vilnius";
            userInit.Email = "vilnius@pricingtool.eu";
            userInitPwd = "Vilnius56.";
            chkUser = UserManager.Create(userInit, userInitPwd);

            //Add default User to Role Preview   
            if (chkUser.Succeeded)
            {
                var result1 = UserManager.AddToRole(userInit.Id, "Preview");
            }

            userInit = new ApplicationUser();
            userInit.UserName = "riga";
            userInit.Email = "vilnius@pricingtool.eu";
            userInitPwd = "Riga89.";
            chkUser = UserManager.Create(userInit, userInitPwd);

            //Add default User to Role Preview   
            if (chkUser.Succeeded)
            {
                var result1 = UserManager.AddToRole(userInit.Id, "Preview");
            }


            userInit = new ApplicationUser();
            userInit.UserName = "modlin";
            userInit.Email = "modlin@pricingtool.eu";
            userInitPwd = "Modlin112.";
            chkUser = UserManager.Create(userInit, userInitPwd);

            //Add default User to Role Preview   
            if (chkUser.Succeeded)
            {
                var result1 = UserManager.AddToRole(userInit.Id, "Preview");
            }

            userInit = new ApplicationUser();
            userInit.UserName = "athens";
            userInit.Email = "athens@pricingtool.eu";
            userInitPwd = "Athens112.";
            chkUser = UserManager.Create(userInit, userInitPwd);

            //Add default User to Role Preview   
            if (chkUser.Succeeded)
            {
                var result1 = UserManager.AddToRole(userInit.Id, "Preview");
            }
        }
    }
}
