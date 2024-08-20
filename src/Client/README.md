# tdl-client-dotnet

Use as:
```dotnet
QueueBasedImplementationRunner runner = new QueueBasedImplementationRunner.Builder()
        .setConfig(getRunnerConfig())
        .withSolutionFor("sum", entry::sum)
        .withSolutionFor("hello", entry::hello)
        .withSolutionFor("array_sum", entry::arraySum)
        .withSolutionFor("int_range", entry::intRange)
        .withSolutionFor("fizz_buzz", entry::fizzBuzz)
        .withSolutionFor("checkout", entry::checkout)
        .withSolutionFor("checklite", entry::checklite)
        .create();

ChallengeSession.forRunner(runner)
        .withConfig(getConfig())
        .withActionProvider(new UserInputAction(args))
        .start();
```