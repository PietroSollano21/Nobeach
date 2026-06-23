// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
	var loginModal = document.getElementById('loginModal');
	if (loginModal) {
		loginModal.addEventListener('shown.bs.modal', function () {
			var el = document.getElementById('loginEmail');
			if (el) el.focus();
		});
	}

	var cadastroModal = document.getElementById('cadastroModal');
	if (cadastroModal) {
		cadastroModal.addEventListener('shown.bs.modal', function () {
			var el = document.getElementById('cadNome');
			if (el) el.focus();
		});
	}

	// CTAs do hero: botões funcionais
	var ctaAgendar = document.getElementById('ctaAgendar');
	var ctaEntrar = document.getElementById('ctaEntrar');
	var chooseAuthModal = document.getElementById('chooseAuthModal');

	if (ctaAgendar) {
		ctaAgendar.addEventListener('click', function (e) {
			e.preventDefault();
			if (chooseAuthModal) {
				var modal = new bootstrap.Modal(chooseAuthModal, {});
				modal.show();
			} else {
				window.location.href = '/Home/Agenda';
			}
		});
	}

	if (ctaEntrar) {
		ctaEntrar.addEventListener('click', function (e) {
			e.preventDefault();
			if (chooseAuthModal) {
				var modal = new bootstrap.Modal(chooseAuthModal, {});
				modal.show();
			}
		});
	}

	// Botões do modal de escolha
	var chooseEntrar = document.getElementById('chooseEntrar');
	var chooseCadastrar = document.getElementById('chooseCadastrar');

	if (chooseEntrar) {
		chooseEntrar.addEventListener('click', function () {
			if (chooseAuthModal) {
				var modal = bootstrap.Modal.getInstance(chooseAuthModal);
				if (modal) modal.hide();
			}
			if (loginModal) {
				var loginModalBS = new bootstrap.Modal(loginModal, {});
				loginModalBS.show();
			}
		});
	}

	if (chooseCadastrar) {
		chooseCadastrar.addEventListener('click', function () {
			if (chooseAuthModal) {
				var modal = bootstrap.Modal.getInstance(chooseAuthModal);
				if (modal) modal.hide();
			}
			if (cadastroModal) {
				var cadastroModalBS = new bootstrap.Modal(cadastroModal, {});
				cadastroModalBS.show();
			}
		});
	}

	// Link "Cadastre-se" no login modal
	var openCadastroFromLogin = document.getElementById('openCadastroFromLogin');
	if (openCadastroFromLogin) {
		openCadastroFromLogin.addEventListener('click', function (e) {
			e.preventDefault();
			if (loginModal) {
				var modal = bootstrap.Modal.getInstance(loginModal);
				if (modal) modal.hide();
			}
			if (cadastroModal) {
				var cadastroModalBS = new bootstrap.Modal(cadastroModal, {});
				cadastroModalBS.show();
			}
		});
	}

	// Navbar shrink on scroll
	var navbar = document.querySelector('.navbar');
	window.addEventListener('scroll', function () {
		if (window.scrollY > 10) {
			navbar.classList.add('scrolled');
		} else {
			navbar.classList.remove('scrolled');
		}
	});
});

// Validação simples de email para formulários (modal e página)
function isValidEmail(email) {
	var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
	return re.test(String(email).toLowerCase());
}

function attachEmailValidation(formId, emailSelector, feedbackId) {
	var form = document.getElementById(formId);
	if (!form) return;
	form.addEventListener('submit', function (e) {
		var emailEl = form.querySelector(emailSelector);
		if (!emailEl) return;
		var val = emailEl.value || '';
		if (!isValidEmail(val)) {
			e.preventDefault();
			emailEl.classList.add('is-invalid');
			var fb = document.getElementById(feedbackId);
			if (fb) fb.style.display = 'block';
			if (emailEl) emailEl.focus();
		} else {
			emailEl.classList.remove('is-invalid');
			var fb2 = document.getElementById(feedbackId);
			if (fb2) fb2.style.display = 'none';
		}
	});
}

attachEmailValidation('modalCadastroForm', 'input[name="email"]', 'cadEmailFeedback');
attachEmailValidation('modalLoginForm', 'input[name="email"]', 'loginEmailFeedback');
attachEmailValidation('loginForm', 'input[name="email"]', 'loginEmailFeedback');
attachEmailValidation('cadastroPageForm', 'input[name="email"]', 'cadEmailFeedback');
