# 💳 Sistema Bancário – Aplicação de Console em C#

Este projeto é uma aplicação de console desenvolvida em C#, com foco em organização de código, modelagem de domínio e boas práticas de engenharia de software.

O sistema simula operações básicas de um banco, priorizando uma estrutura clara e preparada para evoluções futuras.

## 🎯 Objetivos do Projeto
- Praticar Programação Orientada a Objetos com um domínio real
- Modelar entidades e seus relacionamentos
- Aplicar encapsulamento e imutabilidade
- Garantir validação de entradas e regras de negócio
- Separar responsabilidades entre UI, domínio e armazenamento
- Construir uma base sólida para projetos mais complexos

## 🧱 Modelagem de Domínio
- **Client**: representa uma pessoa real, com identidade própria
- **BankAccount**: representa uma conta bancária associada a um cliente

Cada conta bancária possui obrigatoriamente um cliente como proprietário, reforçando uma modelagem mais próxima de sistemas reais.

## 🛠️ Conceitos Aplicados
- Programação Orientada a Objetos (OOP)
- Encapsulamento e propriedades somente leitura (get-only)
- Uso correto de `decimal` para valores monetários
- Interfaces para desacoplamento (`IAccountStorage`)
- Separação de responsabilidades (UI, domínio e dados)
- Validação de entradas do usuário e regras de domínio
- Uso de coleções genéricas e LINQ

## 🚀 Possíveis Evoluções Futuras
- Permitir que um cliente possua múltiplas contas
- Persistência de dados em arquivo ou banco de dados
- Criação de uma camada de serviços para regras de negócio
- Implementação de testes unitários
- Evolução para uma arquitetura mais limpa

## ▶️ Como Executar
1. Clone o repositório
2. Abra o projeto no Visual Studio ou VS Code
3. Execute a aplicação
4. Utilize o menu interativo no console

---

📌 Este projeto faz parte do meu processo contínuo de aprendizado em engenharia de software.
