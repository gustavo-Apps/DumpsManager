
# 💾 DumpsManager

**DumpsManager** é uma aplicação desktop desenvolvida em **WPF** para gerenciar e executar arquivos `.sql` (dumps) em bancos de dados, sejam eles locais ou em nuvem. As conexões são armazenadas de forma prática em um banco de dados **SQLite** local.

## 🚀 Funcionalidades

- Execução automatizada de múltiplos dumps `.sql` e `.txt`
- Suporte a múltiplas conexões de banco de dados
- Armazenamento das conexões em SQLite
- Interface visual moderna com **WPF**
- Geração de log e visualização do progresso da execução
- Modo de build para produção com instalador via Inno Setup

## 🛠️ Tecnologias Utilizadas

- **WPF (Windows Presentation Foundation)**
- **C# .NET**
- **SQLite** para persistência local
- **Inno Setup** para criação do instalador

## 📁 Estrutura de Pastas

```
DumpsManager/
│
├── Assets/          → Ícones e imagens da aplicação
├── Data/            → Lógica de acesso e recuperação de dados
├── FrmLayout/       → Layouts de formulários (UserControls, etc.)
├── Models/          → Classes de modelos e conexões com banco
├── Pages/           → Telas/páginas da aplicação
├── App.xaml         → Arquivo principal de configuração visual
├── MainWindow.xaml  → Janela principal da aplicação
```

```
Setup/
│
├── Assets/          → Recursos para o instalador
├── Output/          → Saída do build
├── Setup.iss        → Script do Inno Setup para gerar o instalador
```

## 📦 Build e Distribuição

A pasta `Setup` contém tudo o que é necessário para gerar o instalador da aplicação em modo **Release**, facilitando a distribuição e instalação do programa.
