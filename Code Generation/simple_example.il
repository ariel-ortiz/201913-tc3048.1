// CIL example program.
//
// To assemble:
//                 ilasm simple_example.il

.assembly 'example' { }

.class public 'Test' extends ['mscorlib']'System'.'Object' {
    .method public static void 'whatever'() {
        .entrypoint
        ldc.i4 42
        call void class ['mscorlib']'System'.'Console'::'WriteLine'(int32)
        ret
    }
}

