## HangFireSample

Este projeto demonstra como integrar o Hangfire, uma biblioteca popular para processamento de jobs em background no .NET, a uma minimal API do ASP.NET Core. Ele mostra como configurar, disparar e monitorar jobs do tipo fire-and-forget, com delay, recorrentes e de continuação, usando o dashboard do Hangfire.

### Funcionalidades

- **Fire-and-Forget**: executa uma tarefa imediatamente, disparada por um endpoint da API.
- **Jobs com delay**: agenda uma tarefa para rodar após um tempo especificado.
- **Jobs recorrentes**: um job registrado na inicialização que roda a cada minuto.
- **Continuations**: executa uma tarefa após a conclusão de outro job.
- **Monitoramento pelo dashboard**: monitoramento e gerenciamento em tempo real dos jobs em background.
- Os jobs são classes resolvidas via DI (`SampleJobs`), em vez de métodos estáticos, permitindo dependências como `ILogger<T>`.

### Requisitos

- .NET 10.0 SDK ou superior
- Hangfire 1.8 ou superior

### Instalação

1. Clone o repositório:
    ```bash
    git clone https://github.com/denisams/HangFireSample.git
    ```
2. Acesse o diretório do projeto:
    ```bash
    cd HangFireSample
    ```
3. Restaure as dependências:
    ```bash
    dotnet restore
    ```
4. Execute o projeto:
    ```bash
    dotnet run --project HangFireLearn.api
    ```

### Uso

Com o projeto em execução, acesse:

- `/swagger` — Swagger UI, listando os endpoints disponíveis em `/api/jobs` para disparar jobs sob demanda.
- `/hangfire` — o dashboard do Hangfire, para monitorar e inspecionar a execução dos jobs.

Endpoints disponíveis (todos `POST`):

| Endpoint | Descrição |
| --- | --- |
| `/api/jobs/fire-and-forget` | Enfileira um job que roda imediatamente. |
| `/api/jobs/fire-and-forget-with-error` | Enfileira um job que sempre lança uma exceção, útil para observar o comportamento de retry do Hangfire no dashboard. |
| `/api/jobs/delayed?delay=00:01:00` | Agenda um job para rodar após o `TimeSpan` informado (padrão de 1 minuto). |
| `/api/jobs/continuation` | Enfileira um job pai e um job filho que só roda após o pai ser concluído. |
| `/api/jobs/notification?message=Hello` | Enfileira um job com um parâmetro passado na requisição. |
| `/api/jobs/batch?itemCount=5` | Enfileira um job que processa um lote de itens um a um, registrando o progresso. |
| `/api/jobs/long-running` | Enfileira um job simulando uma tarefa de múltiplas etapas e vários segundos. |
| `/api/jobs/heartbeat/start` | (Re)inicia o job de heartbeat "a cada 10 segundos" (veja abaixo). |
| `/api/jobs/heartbeat/stop` | Para o job de heartbeat após o tick atual de 10 segundos. |

Um job recorrente (`hello-recurring`) é registrado na inicialização e roda a cada minuto — não é necessário dispará-lo manualmente.

#### O job de heartbeat "a cada 10 segundos"

Os jobs recorrentes do Hangfire baseados em cron (`RecurringJob.AddOrUpdate`) não conseguem rodar em intervalos menores que 1 minuto — o próprio cron não tem campo de segundos. O `SampleJobs.Heartbeat` contorna isso da forma recomendada pela própria documentação do Hangfire para jobs com intervalo abaixo de um minuto: cada execução reagenda a si mesma 10 segundos depois via `BackgroundJobClient.Schedule`, enquanto `SampleJobs.TryEnableHeartbeat`/`DisableHeartbeat` não o desativarem. Ele é iniciado automaticamente na inicialização da aplicação e pode ser parado/reiniciado pelos endpoints acima. Como o exemplo usa `Hangfire.MemoryStorage`, todo o estado (incluindo essa cadeia) é resetado a cada reinicialização, então é seguro dispará-lo incondicionalmente no `Program.cs`.

Note que `AddHangfireServer` também reduz o `SchedulePollingInterval` para 2 segundos (o padrão é 15s) — caso contrário, jobs agendados/com delay, incluindo esse heartbeat, só seriam processados a cada 15 segundos, independentemente do delay solicitado.

### Segurança do dashboard

O Hangfire não restringe o acesso ao dashboard por padrão. Este exemplo só permite acesso a `/hangfire` no ambiente `Development` (veja `HangfireDashboardAuthorizationFilter`). Substitua esse filtro por autenticação/autorização real antes de fazer deploy em qualquer outro ambiente.

### Executando com Docker

```bash
docker build -t hangfiresample .
docker run --rm -it -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development hangfiresample
```

### Contribuindo

Sinta-se livre para fazer um fork deste repositório e enviar pull requests. Para mudanças maiores, abra uma issue primeiro para discutir o que você gostaria de alterar.
