# 🎮 Pokémon Unity Battle System

Este projeto foi desenvolvido como um teste técnico com o objetivo de **reproduzir a cena de batalha do Pokémon Emerald**, integrando dados reais consumidos da **[PokeAPI](https://pokeapi.co/)**.

O foco principal foi o uso de componentes de layout da Unity e a implementação de arquitetura assíncrona para consumo de dados web, além de funcionalidades extras como gerenciamento de time e telas de detalhes.

## ✨ Funcionalidades Implementadas

### ⚔️ Cenário de Batalha
Recriação da interface de batalha clássica atendendo aos requisitos do teste:
* **Integração PokeAPI:** Exibição dinâmica de nome e sprites carregados da web;
* **Moveset Dinâmico:** Listagem de até 4 movimentos por Pokémon;
* **Sprites Dinâmicos:** Frente para o inimigo, costas para aliado;
* **Detalhes dos Moves:** Exibição `Tipo` e `PP` atualizados via API.

### 🎒 Sistema de party & extras
Além do escopo básico, foram implementadas as funcionalidades extras sugeridas e melhorias de UX:
* **Tela de Time (Party):** Visualização da equipe completa com opção de troca de Pokémon (Switch);
* **Menu de Contexto:** Sistema de pop-up na Party para escolher entre "Switch In" ou "Summary".
* **Tela de Summary:** Visualização detalhada de summary do pokémon.

### 🔄 Polimento e arquitetura
* **Loading screen:** Cena dedicada para pré-carregar dados pesados (JSON/Sprites) e garantir fluidez ao entrar na batalha.
* **Transições de cena:** Sistema visual de "slide" para transitar suavemente entre Batalha, Party e Summary.
* **Cache de assets:** Otimização para armazenar texturas em memória e evitar re-downloads.
* **Hover effects:** Feedback visual interativo ao selecionar botões e slots.

## 🛠️ Tecnologias Utilizadas
* **Engine:** Unity, versão 6000.0.58f2
* **Linguagem:** C#
* **API:** [PokeAPI](https://pokeapi.co/) (REST)

## 📸 Screenshots
<img width="554" height="311" alt="Captura de Tela 2025-12-26 às 03 43 26" src="https://github.com/user-attachments/assets/7f6e572f-2848-4617-b455-33c13e6af7cb" />

<img width="553" height="312" alt="Captura de Tela 2025-12-26 às 03 44 23" src="https://github.com/user-attachments/assets/4d895dbb-4d7a-4787-96fb-6ff6407257cd" />

<img width="553" height="312" alt="Captura de Tela 2025-12-26 às 03 45 02" src="https://github.com/user-attachments/assets/d4d0d638-2e96-4e6c-8429-cd81b5549943" />

<img width="552" height="309" alt="Captura de Tela 2025-12-26 às 03 45 29" src="https://github.com/user-attachments/assets/5239676c-40fe-458f-9561-2fcde13f8892" />

## 🚀 Como rodar o projeto

Para garantir a visualização e funcionamento corretos da interface (UI), siga as configurações abaixo:

1.  **Clone o repositório** para sua máquina local.
2.  Abra o projeto via **Unity Hub**.
3.  **Configuração de Resolução:**
    * Defina a resolução da janela de Game para **Full HD (1920x1080)**. Isso é crucial para que a UI se adapte corretamente.
4.  **Iniciar o Jogo:**
    * Abra a cena **`LoadingScene`** (`Assets/Scenes/LoadingScene.unity`).
    * Dê **Play**. O jogo deve começar por esta cena para garantir que todos os dados da API sejam carregados antes da batalha iniciar.

---
*Desenvolvido como parte do teste técnico para a BSATech.*
