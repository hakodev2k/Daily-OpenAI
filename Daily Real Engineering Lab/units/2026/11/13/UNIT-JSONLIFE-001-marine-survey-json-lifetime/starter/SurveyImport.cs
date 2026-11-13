using System.Text.Json;
public static class SurveyImport {
 public static JsonElement Read(string payload) {
  using var document = JsonDocument.Parse(payload);
  return document.RootElement.GetProperty("station");
 }
}
