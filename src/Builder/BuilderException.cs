namespace REBoP.Builder;

public class BuilderException(string message) : Exception(message);

public class MissingRequiredFieldBuilderException(string fieldName)
    : BuilderException($"A {fieldName} is required, but none was set");
