using System;
using System.Collections.Generic;

public class GeneratorExample {

    public static IEnumerable<int> SimpleExample() {
        yield return 4;
        yield return 8;
        yield return 15;
        yield return 16;
    }

    public static IEnumerable<int> Pow2() {
        var accum = 1;
        while (true) {
            yield return accum;
            checked {
                accum *= 2;
            }
        }
    }

    public static void Main() {
        var e = SimpleExample().GetEnumerator();
        while (e.MoveNext()) {
            Console.WriteLine(e.Current);
        }

        Console.WriteLine();

        e = Pow2().GetEnumerator();
        int i;
        do {
            e.MoveNext();
            i = e.Current;
            Console.WriteLine(i);
        } while (i < 10000); 
    }
}