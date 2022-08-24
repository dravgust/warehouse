export function ProviderName(providerId) {
  switch (providerId) {
    case 1:
      return "Electra";
    case 2:
      return "Dolav";
    case 3:
      return "Meitav";
    case 4:
      return "Tel-Aviv University";
    case 1000:
      return "Vayosoft";
    default:
      return "n/a";
  }
}
