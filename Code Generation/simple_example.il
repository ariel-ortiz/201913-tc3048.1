// CIL example program.
//
// To assemble:
//                 ilasm simple_example.il

.assembly 'example' { }

.class public 'Test' extends ['mscorlib']'System'.'Object' {
    .method public static void 'whatever'() {
        .entrypoint
        ldc.i4.3
        ldc.i4.6
        mul
        neg
        ldc.i4.2
        ldc.i4.3
        add
        ldc.i4.2
        rem
        add
        call void class ['mscorlib']'System'.'Console'::'WriteLine'(int32)
        ret
    }
}

