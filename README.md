# 15-pl-04
Implementacja klastra obliczeniowego na Inżynierię Oprogramowania 2.

### Konwencja nazewnicza & coding guidlines

Obowiązuje konwencja C# ze strony MSDN. Zapoznanie się z poniższym jest w obowiązkowe - kod nietrzymający się konwencji nie będzie zmergowany do mastera, dopóki nie zostanie poprawiony. 

1. https://msdn.microsoft.com/en-us/library/ms229002(v=vs.110).aspx
2. https://msdn.microsoft.com/en-us/library/ff926074.aspx
3. Kwestie niespecyfikowane przez powyższe:
  * Prywatne pola klas: `_camelCase`
  * Stałe `const`, niezależnie od widoczności: `PascalCase`*
  
_gdy jest wybór, **zawsze** wybieramy `const` zamiast `static readonly`_

Ponadto: nazwy branchy muszą zawierać informacje o właścicielu i funkcjonalności jaką obejmują. Przykłady poniżej (zwróćcie uwagę na kapitalizację literek): 

* _pie/refactoring_
* _leo/serialization_
* _rog/comClientUnitTests_
* _kut/componentRegistration_

### Code review i mergowanie

Jako Team Leader mergować (na mastera) będę wyłącznie ja (Mateusz). Chciałbym, żeby cały proces wyglądał w następujący sposób:

1. Jan Kowalski **kończy*** funkcjonalność nad którą pracował na swojej branchy _kow/helloWorld_.
2. Kowalski informuje (na przykład na Facebooku) o potrzebie zmergowania swojego kodu do mastera.
3. Rozpoczyna się _code review_ trwający od kilku godzin do max kilku dni (w zależności od ilości nowego kodu na dodawanej branchy). W tym czasie każdy dodaje  komentarze na GitHubie w tych miejscach w kodzie, w których są błędy do poprawienia. 
4. Kowalski poprawia błędy. Jeżeli było ich dużo i były znaczące, robimy drugi _code review_.
5. Jeśli wszystko jest OK, Team Leader merguje branchę do mastera, a Kowalski robi nową branchę, gdzie zajmuje się czymś innym.

*w praktyce kluczowe funkcjonalności będą zapewne mergowane niekompletne (gdy ich brak blokuje pracę innej osoby). 

# FAQ
