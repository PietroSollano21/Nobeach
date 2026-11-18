# 📝 No Beach

> Este projeto é um site de uma Arena de Beach Tennis, com foco total no agendamento de quadras e na administração da arena

---

## 📌 Sobre o projeto

O projeto consiste em uma aplicação web desenvolvida para realizar o agendamento de quadras em uma arena de Beach Tennis, permitindo que os clientes consultem horários disponíveis e realizem suas reservas de forma prática.

A aplicação também conta com um painel administrativo, desenvolvido para facilitar o gerenciamento do estabelecimento, permitindo ao administrador acompanhar e controlar os agendamentos e a disponibilidade das quadras.

Entre os recursos administrativos, destaca-se o gerenciamento de folgas, que permite ao administrador definir datas em que a arena não estará em funcionamento. Ao cadastrar uma folga, todos os agendamentos para aquela data são bloqueados, impedindo que novas reservas sejam realizadas.

O sistema foi desenvolvido com o objetivo de oferecer uma solução prática para automatizar o processo de agendamento e facilitar a administração de uma arena esportiva.

## 🎯 Objetivos

- Aprendizado de desenvolvimento web
- Aprendizado de lógica de programação
- Lidar com problemas e necessidades reais
- Aprendizado com uso de APIs
- Aprendizado de uso de banco de dados

## 🚀 Funcionalidades

- [ ] Sistema de agendamento
- [ ] Sistema de cancelamento
- [ ] Gerenciamento de folgas
- [ ] Sistema de login e cadastro
- [ ] Sistema de recuperação e redefinição de senha
- [ ] Sistema de confirmação e troca de email

## 🛠️ Tecnologias utilizadas

\<!-- Informe as linguagens, frameworks, bibliotecas e ferramentas utilizadas. -->

| Tecnologia   | Finalidade              |
| ------------ | ----------------------- |
| C# | Back-End |
| JavaScript | Front-End |
| CsHtml | Front-End |
| ASP Net Core | Framework de desenvolvimento |
| .Net | Biblioteca |
| Resend | API de envio de email |
| MySql | Banco de dados |

## 🏗️ Arquitetura do projeto

Os arquivos foram separados conforme ás responsabilidades de cada um, mantendo uma organização para o seu desenvolvimento e em alguns casos, correção

### Estrutura de diretórios

```text
Nobeach.Web/
│
├── Controllers/
│   ├── AdmControllers.cs
|   ├── AgendamentoControllers.cs
|   ├── AuthControllers.cs
|   ├── HomeControllers.cs
│   └── UsuarioControllers.cs
|
│
├── Data/
│   ├── AppDbContext.cs
│   └── Conexao.cs
|
|
├── DTOS/
|   └──  Login.cs
│
|
├── Enum/
|   └── Esporte.cs
|
|
├── Models/
|   ├── Agendamento.cs
|   ├── Diaquadras.cs
|   ├── Quadras.cs
|   └── Usuario.cs
|   
├── Repositories/
|   ├── AgendamentoRepositories.cs
|   └── UsuarioRepositories.cs
|
|
├── Services/
|   ├── Interfaces/
|   |    └──IEmailService.cs
|   └── EmailService.cs
|
|
├── Views/
|   ├── Adm/
|   |    ├── Admin.cshtml
|   |    └── ConfigurarDias.cshtml
|   ├── Home/
|   |    ├── Agenda.cshtml
|   |    ├── Index.cshtml
|   |    └── Privacy.cshtml
|   ├── Shared/
|   |    ├── Error.cshtml
|   |    ├── _Layout.cshtml
|   |    ├── _Layout.cshtml.css
|   |    └── _ValidationScriptsPartial.cshtml
|   ├── Usuario/
|   |    ├── AlterarEmail.cshtml
|   |    ├── Cadastro.cshtml
|   |    ├── Confirmado.cshtml
|   |    ├── Email.cshtml
|   |    ├── EsqueciSenha.cshtml
|   |    ├── Expirado.cshtml
|   |    ├── Login.cshtml
|   |    └── RedefinaSenha.cshtml
|   └── _ViewImports.cshtml
├── wwwroot/
|   ├── css
|   |     └── site.css
|   └── js/
|       └── site.js
└── README.md
```

## 🖥️ Demonstração


### Tela inicial
<img width="1294" height="613" alt="image" src="https://github.com/user-attachments/assets/07204cb3-7d07-444a-a553-5f3abc14c638" />
<img width="1294" height="613" alt="image" src="https://github.com/user-attachments/assets/a477be0c-a4d2-491a-884c-0c9ab2392bef" />
<img width="1307" height="624" alt="image" src="https://github.com/user-attachments/assets/f22ddc41-df16-4871-bf21-0504bbdfa3f1" />


### Funcionalidade principal
<img width="1307" height="624" alt="image" src="https://github.com/user-attachments/assets/74b424d0-5883-4b08-8a00-a086e016da26" />
<img width="1307" height="624" alt="image" src="https://github.com/user-attachments/assets/ca7d96f6-fddf-44ad-ac2b-03f34befa360" />
<img width="1307" height="624" alt="image" src="https://github.com/user-attachments/assets/9c037951-e8df-4150-973e-42d0a7927273" />



## 🗄️ Banco de dados

O banco de dados utilizado foi o MySql, utilizei ele por mais familiaridade e por mais facilidade de uso

**Banco utilizado:** nobeachdb

### Principais tabelas

| Tabela     | Descrição   |
| ---------- | ----------- |
| Diaquadras | Verifica se a quadra está disponível |
| Quadras | Registra as quadras, verificando sua disponibilidade |
| Usuarios | Registra os usuários |
| agendamento | Registra agendamentos e seus status |

### Relacionamentos

Todas as tabelas estão relacionadas, pois cada uma preenche uma parte do agendamento da seguinte forma
> A tabela usuario registra quando um usuário é criado, guardando suas informações
> Quando o usuário vai fazer um agendamento, ele consulta a tabela de quadras, que verifica se a quadra está reservada ou indisponível naquela data ou horario
> A tabela Diaquadras registra quais dias as quadras vão funcionar, estando relacionada com o sistema de gerenciamento de folgas

## 🔌 API

### Resend

- **Objetivo:** Usada para enviar emails 
- **Utilização:** Utilizada para enviar os emails de confirmação de email, para validar o usuário, e em alguns casos, trocar o email ou a senha
- **Funcionamento:** Quando algum usuário cria uma conta, ele recebe um email de confirmação, e ao clicar no link do email, ele é enviado para a página principal, validando sua conta. Caso o usuário vá deixar de utilizar aquele email, ele tem a opção de troca de email. Caso tenha esquecido a senha da conta, ele posssui a opção de trocar de senha ou recuperar sua senha

## 🔮 Melhorias futuras

- [ ] Integração de API de pagamentos
- [ ] Adicionar o local da arena
- [ ] Adicionar a página de perfil do usuário com opção de logout

## 👨‍💻 Autor

**Pietro Sollano**

- GitHub: [https://github.com/PietroSollano21]
- LinkedIn: [www.linkedin.com/in/pietro-sollano-5991b73a7]
- E-mail: pietrosollano21@gmail.com

---

## 📄 Link do site para visualização
- LInk: [ nobeach.com.br ]

