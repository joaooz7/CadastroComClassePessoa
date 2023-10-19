using AjudaCertaCadastro.ViewModels.Usuarios;

namespace AjudaCertaCadastro.Views.Usuarios;

public partial class TelaInicialView : ContentPage
{
	UsuarioViewModel usuarioViewModel;
	public TelaInicialView()
	{
		InitializeComponent();
		usuarioViewModel = new UsuarioViewModel();
		BindingContext = usuarioViewModel;
	}
}