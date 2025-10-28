# 🏥 HealthTracker - Sistema de Rastreamento de Atividades de Saúde
- Rafael De Almeida Sigoli (RM554019)
- Giovanna Franco Gaudino Rodrigues (RM553701)
- Rafael Jorge Del Padre (rm552765)
- Lucas Bertolassi Iori (rm553183)

## 📖 Descrição do Projeto
Sistema completo desenvolvido em C# .NET 8 para monitoramento e gerenciamento de atividades de saúde pessoal. Permite registrar, analisar e acompanhar diversos tipos de atividades físicas e de bem-estar com interface intuitiva e relatórios detalhados. 💪📊

## 🚀 Funcionalidades Principais

### 📝 Registro e Gerenciamento
- Adicionar Registros: Registre diferentes tipos de atividades de saúde
- Listar Registros: Visualize todos os registros com filtros por data, tipo ou recência
- Editar Registros: Modifique atividades existentes
- Excluir Registros: Remova registros indesejados
- Busca Avançada: Encontre registros por tipo de atividade ou texto nas notas

### 📈 Análises e Estatísticas
- 📊 Estatísticas Detalhadas: Métricas completas por tipo de atividade
- 📈 Análise de Tendências: Acompanhe a evolução das suas métricas
- ✅ Conformidade: Compare seus resultados com faixas recomendadas
- 🔗 Correlações: Identifique relações entre diferentes atividades
- 📅 Relatórios Periódicos: Gere relatórios diários, semanais, mensais e anuais

### 🧠 Sistema Inteligente
- Recomendações Personalizadas: Sugestões baseadas em suas métricas
- Alertas de Conformidade: Notificações quando estiver acima/abaixo do recomendado
- Insights Automáticos: Análises inteligentes do seu progresso
- Exportação de Dados: Exporte relatórios em CSV e JSON

## 💻 Tecnologias Utilizadas
- .NET 8: Framework principal
- C# 12: Linguagem de programação
- Arquitetura em Camadas: Separação de responsabilidades
- Repository Pattern: Padrão de acesso a dados
- Console Application: Interface baseada em terminal
- System.Text.Json: Serialização de dados

## 📁 Estrutura do Projeto
```
HealthTracker/
├── Controllers/ # Controladores MVC
├── Services/ # Serviços de aplicação e domínio
├── DTOs/ # Data Transfer Objects
├── Enums/ # Enumerações do sistema
├── Utils/ # Utilitários e helpers
├── Models/ # Modelos de domínio
├── Interfaces/ # Contratos e interfaces
```

## 🏃‍♂️ Tipos de Atividades Suportadas
1. 💪 Exercício - Atividades físicas gerais
2. 💧 Água - Consumo de água diário
3. 😴 Sono - Controle de horas de descanso
4. 🧘‍♂️ Meditação - Práticas de mindfulness
5. 🚶‍♂️ Caminhada - Atividades de caminhada
6. 🤸‍♂️ Alongamento - Exercícios de flexibilidade
7. 🧘‍♀️ Yoga - Práticas de yoga
8. 🏊‍♂️ Natação - Atividades aquáticas
9. 🏃‍♀️ Corrida - Exercícios de corrida
10. 🚴‍♂️ Ciclismo - Atividades ciclísticas

## ⚙️ Instalação e Configuração

### Pré-requisitos
- .NET 8 SDK ou superior
- Terminal/Command Prompt
- 50MB de espaço em disco

### 🚀 Passos de Instalação
1. Baixe o projeto ou clone o repositório
2. Navegue até o diretório do projeto
3. Execute o comando: `dotnet build`
4. Execute a aplicação: `dotnet run`

### 🔧 Configuração Inicial
O sistema cria automaticamente:
- Diretório de dados local
- Estrutura de pastas para exportações
- Arquivos de configuração padrão

## 📖 Como Usar

### 🏠 Menu Principal
Ao iniciar a aplicação, você verá o menu com as opções:

1. Adicionar Registro: Crie novos registros de atividades
2. Listar Registros: Visualize atividades existentes
3. Gerenciar Registros: Edite ou exclua registros
4. Estatísticas Detalhadas: Veja análises completas
5. Análise de Tendências: Acompanhe sua evolução
6. Relatórios: Gere relatórios personalizados
7. Buscar Registros: Encontre registros específicos
8. informações do Sistema: Detalhes técnicos
9. Sair: Encerre a aplicação

### ➕ Adicionando um Registro
1. Selecione a opção 1 no menu principal
2. Escolha o tipo de atividade (digite o número ou nome)
3. Informe a data no formato dd/MM/aaaa
4. Digite o valor da atividade
5. Adicione notas opcionais
6. Defina a intensidade (1-10)

### 📊 Gerando Estatísticas
1. selecione a opção 4 no menu
2. Escolha o tipo de atividade desejado
3. Defina o período de análise
4. Visualize o relatório completo com:
   - Totais e médias
   - Valores máximo e mínimo
   - Taxa de conformidade
   - Tendência de evolução
   - Recomendações personalizadas

## 🔧 Recursos Avançados

### ✅ Sistema de Conformidade
Compara seus resultados com faixas recomendadas e fornece alertas quando estiver acima/abaixo do ideal, com sugestões de ajustes baseadas em métricas. ⚠️📊

### 📈 Análise de Tendências
Identifica padrões de comportamento, mostra evolução temporal e calcula correlações entre atividades diferentes. 🔍📈

### 📤 Exportação de Dados
Exporte relatórios em CSV para uso em planilhas ou formato JSON para integrações com outros sistemas. Os arquivos são salvos no diretório "Exports". 📄💾

## 📅 Formato de Datas
O sistema utiliza o formato brasileiro: **dd/MM/aaaa**  
📅 Exemplos: 01/01/2025, 15/03/2025, 31/12/2025

## 💡 Sistema de Recomendações
O sistema fornece recomendações baseadas em padrões de saúde estabelecidos:
- Faixas ideais para cada tipo de atividade
- Alertas quando os valores estão fora do recomendado
- Sugestões de melhoria baseadas em tendências

## 🏗️ Arquitetura do Sistema
O projeto utiliza uma arquitetura em camadas baseada nos princípios do Domain-Driven Design (DDD) e Clean Architecture, com as seguintes características:

### 🎯 Padrões Arquiteturais
- **Clean Architecture**: Separação clara entre domínio, aplicação e infraestrutura
- **Domain-Driven Design (DDD)**: Modelagem centrada no domínio de saúde e atividades
- **Repository Pattern**: Abstração do acesso a dados
- **Service Layer**: Camada de serviços para lógica de negócio
- **DTO Pattern**: Objetos de transferência de dados para comunicação entre camadas
- **MVC Pattern**: Separação entre Model, View e Controller

### 🏢 Camadas da Arquitetura
1. **Domain Layer**: Entidades de domínio e regras de negócio fundamentais
2. **Application Layer**: Casos de uso e serviços de aplicação
3. **Infrastructure Layer**: Implementações concretas (armazenamento, serviços externos)
4. **Presentation Layer**: Interface do usuário (Console Application)

### ✨ Princípios de Design
- ⭐ SOLID Principles
- 💉 Dependency Injection
- 🎯 Separation of Concerns
- ✅ Single Responsibility Principle
