/*************************************************************************
Copyright (c) Sergey Bochkanov (ALGLIB project).

>>> SOURCE LICENSE >>>
This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation (www.fsf.org); either version 2 of the
License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

A copy of the GNU General Public License is available at
http://www.fsf.org/licensing/licenses
>>> END OF LICENSE >>>
*************************************************************************/
#pragma warning disable 162
#pragma warning disable 219
using System;

public partial class alglib
{


    /*************************************************************************
    Optimal binary classification

    Algorithms finds optimal (=with minimal cross-entropy) binary partition.
    Internal subroutine.

    INPUT PARAMETERS:
        A       -   array[0..N-1], variable
        C       -   array[0..N-1], class numbers (0 or 1).
        N       -   array size

    OUTPUT PARAMETERS:
        Info    -   completetion code:
                    * -3, all values of A[] are same (partition is impossible)
                    * -2, one of C[] is incorrect (<0, >1)
                    * -1, incorrect pararemets were passed (N<=0).
                    *  1, OK
        Threshold-  partiton boundary. Left part contains values which are
                    strictly less than Threshold. Right part contains values
                    which are greater than or equal to Threshold.
        PAL, PBL-   probabilities P(0|v<Threshold) and P(1|v<Threshold)
        PAR, PBR-   probabilities P(0|v>=Threshold) and P(1|v>=Threshold)
        CVE     -   cross-validation estimate of cross-entropy

      -- ALGLIB --
         Copyright 22.05.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void dsoptimalsplit2(double[] a, int[] c, int n, out int info, out double threshold, out double pal, out double pbl, out double par, out double pbr, out double cve)
    {
        info = 0;
        threshold = 0;
        pal = 0;
        pbl = 0;
        par = 0;
        pbr = 0;
        cve = 0;
        bdss.dsoptimalsplit2(a, c, n, ref info, ref threshold, ref pal, ref pbl, ref par, ref pbr, ref cve);
        return;
    }

    /*************************************************************************
    Optimal partition, internal subroutine. Fast version.

    Accepts:
        A       array[0..N-1]       array of attributes     array[0..N-1]
        C       array[0..N-1]       array of class labels
        TiesBuf array[0..N]         temporaries (ties)
        CntBuf  array[0..2*NC-1]    temporaries (counts)
        Alpha                       centering factor (0<=alpha<=1, recommended value - 0.05)
        BufR    array[0..N-1]       temporaries
        BufI    array[0..N-1]       temporaries

    Output:
        Info    error code (">0"=OK, "<0"=bad)
        RMS     training set RMS error
        CVRMS   leave-one-out RMS error

    Note:
        content of all arrays is changed by subroutine;
        it doesn't allocate temporaries.

      -- ALGLIB --
         Copyright 11.12.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void dsoptimalsplit2fast(ref double[] a, ref int[] c, ref int[] tiesbuf, ref int[] cntbuf, ref double[] bufr, ref int[] bufi, int n, int nc, double alpha, out int info, out double threshold, out double rms, out double cvrms)
    {
        info = 0;
        threshold = 0;
        rms = 0;
        cvrms = 0;
        bdss.dsoptimalsplit2fast(ref a, ref c, ref tiesbuf, ref cntbuf, ref bufr, ref bufi, n, nc, alpha, ref info, ref threshold, ref rms, ref cvrms);
        return;
    }

}
public partial class alglib
{


    /*************************************************************************

    *************************************************************************/
    public class decisionforest
    {
        //
        // Public declarations
        //

        public decisionforest()
        {
            _innerobj = new dforest.decisionforest();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private dforest.decisionforest _innerobj;
        public dforest.decisionforest innerobj { get { return _innerobj; } }
        public decisionforest(dforest.decisionforest obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************

    *************************************************************************/
    public class dfreport
    {
        //
        // Public declarations
        //
        public double relclserror { get { return _innerobj.relclserror; } set { _innerobj.relclserror = value; } }
        public double avgce { get { return _innerobj.avgce; } set { _innerobj.avgce = value; } }
        public double rmserror { get { return _innerobj.rmserror; } set { _innerobj.rmserror = value; } }
        public double avgerror { get { return _innerobj.avgerror; } set { _innerobj.avgerror = value; } }
        public double avgrelerror { get { return _innerobj.avgrelerror; } set { _innerobj.avgrelerror = value; } }
        public double oobrelclserror { get { return _innerobj.oobrelclserror; } set { _innerobj.oobrelclserror = value; } }
        public double oobavgce { get { return _innerobj.oobavgce; } set { _innerobj.oobavgce = value; } }
        public double oobrmserror { get { return _innerobj.oobrmserror; } set { _innerobj.oobrmserror = value; } }
        public double oobavgerror { get { return _innerobj.oobavgerror; } set { _innerobj.oobavgerror = value; } }
        public double oobavgrelerror { get { return _innerobj.oobavgrelerror; } set { _innerobj.oobavgrelerror = value; } }

        public dfreport()
        {
            _innerobj = new dforest.dfreport();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private dforest.dfreport _innerobj;
        public dforest.dfreport innerobj { get { return _innerobj; } }
        public dfreport(dforest.dfreport obj)
        {
            _innerobj = obj;
        }
    }

    /*************************************************************************
    This subroutine builds random decision forest.

    INPUT PARAMETERS:
        XY          -   training set
        NPoints     -   training set size, NPoints>=1
        NVars       -   number of independent variables, NVars>=1
        NClasses    -   task type:
                        * NClasses=1 - regression task with one
                                       dependent variable
                        * NClasses>1 - classification task with
                                       NClasses classes.
        NTrees      -   number of trees in a forest, NTrees>=1.
                        recommended values: 50-100.
        R           -   percent of a training set used to build
                        individual trees. 0<R<=1.
                        recommended values: 0.1 <= R <= 0.66.

    OUTPUT PARAMETERS:
        Info        -   return code:
                        * -2, if there is a point with class number
                              outside of [0..NClasses-1].
                        * -1, if incorrect parameters was passed
                              (NPoints<1, NVars<1, NClasses<1, NTrees<1, R<=0
                              or R>1).
                        *  1, if task has been solved
        DF          -   model built
        Rep         -   training report, contains error on a training set
                        and out-of-bag estimates of generalization error.

      -- ALGLIB --
         Copyright 19.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void dfbuildrandomdecisionforest(double[,] xy, int npoints, int nvars, int nclasses, int ntrees, double r, out int info, out decisionforest df, out dfreport rep)
    {
        info = 0;
        df = new decisionforest();
        rep = new dfreport();
        dforest.dfbuildrandomdecisionforest(xy, npoints, nvars, nclasses, ntrees, r, ref info, df.innerobj, rep.innerobj);
        return;
    }

    /*************************************************************************
    Procesing

    INPUT PARAMETERS:
        DF      -   decision forest model
        X       -   input vector,  array[0..NVars-1].

    OUTPUT PARAMETERS:
        Y       -   result. Regression estimate when solving regression  task,
                    vector of posterior probabilities for classification task.

    See also DFProcessI.

      -- ALGLIB --
         Copyright 16.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void dfprocess(decisionforest df, double[] x, ref double[] y)
    {

        dforest.dfprocess(df.innerobj, x, ref y);
        return;
    }

    /*************************************************************************
    'interactive' variant of DFProcess for languages like Python which support
    constructs like "Y = DFProcessI(DF,X)" and interactive mode of interpreter

    This function allocates new array on each call,  so  it  is  significantly
    slower than its 'non-interactive' counterpart, but it is  more  convenient
    when you call it from command line.

      -- ALGLIB --
         Copyright 28.02.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void dfprocessi(decisionforest df, double[] x, out double[] y)
    {
        y = new double[0];
        dforest.dfprocessi(df.innerobj, x, ref y);
        return;
    }

    /*************************************************************************
    Relative classification error on the test set

    INPUT PARAMETERS:
        DF      -   decision forest model
        XY      -   test set
        NPoints -   test set size

    RESULT:
        percent of incorrectly classified cases.
        Zero if model solves regression task.

      -- ALGLIB --
         Copyright 16.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static double dfrelclserror(decisionforest df, double[,] xy, int npoints)
    {

        double result = dforest.dfrelclserror(df.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    Average cross-entropy (in bits per element) on the test set

    INPUT PARAMETERS:
        DF      -   decision forest model
        XY      -   test set
        NPoints -   test set size

    RESULT:
        CrossEntropy/(NPoints*LN(2)).
        Zero if model solves regression task.

      -- ALGLIB --
         Copyright 16.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static double dfavgce(decisionforest df, double[,] xy, int npoints)
    {

        double result = dforest.dfavgce(df.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    RMS error on the test set

    INPUT PARAMETERS:
        DF      -   decision forest model
        XY      -   test set
        NPoints -   test set size

    RESULT:
        root mean square error.
        Its meaning for regression task is obvious. As for
        classification task, RMS error means error when estimating posterior
        probabilities.

      -- ALGLIB --
         Copyright 16.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static double dfrmserror(decisionforest df, double[,] xy, int npoints)
    {

        double result = dforest.dfrmserror(df.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    Average error on the test set

    INPUT PARAMETERS:
        DF      -   decision forest model
        XY      -   test set
        NPoints -   test set size

    RESULT:
        Its meaning for regression task is obvious. As for
        classification task, it means average error when estimating posterior
        probabilities.

      -- ALGLIB --
         Copyright 16.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static double dfavgerror(decisionforest df, double[,] xy, int npoints)
    {

        double result = dforest.dfavgerror(df.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    Average relative error on the test set

    INPUT PARAMETERS:
        DF      -   decision forest model
        XY      -   test set
        NPoints -   test set size

    RESULT:
        Its meaning for regression task is obvious. As for
        classification task, it means average relative error when estimating
        posterior probability of belonging to the correct class.

      -- ALGLIB --
         Copyright 16.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static double dfavgrelerror(decisionforest df, double[,] xy, int npoints)
    {

        double result = dforest.dfavgrelerror(df.innerobj, xy, npoints);
        return result;
    }

}
public partial class alglib
{


    /*************************************************************************
    k-means++ clusterization

    INPUT PARAMETERS:
        XY          -   dataset, array [0..NPoints-1,0..NVars-1].
        NPoints     -   dataset size, NPoints>=K
        NVars       -   number of variables, NVars>=1
        K           -   desired number of clusters, K>=1
        Restarts    -   number of restarts, Restarts>=1

    OUTPUT PARAMETERS:
        Info        -   return code:
                        * -3, if task is degenerate (number of distinct points is
                              less than K)
                        * -1, if incorrect NPoints/NFeatures/K/Restarts was passed
                        *  1, if subroutine finished successfully
        C           -   array[0..NVars-1,0..K-1].matrix whose columns store
                        cluster's centers
        XYC         -   array which contains number of clusters dataset points
                        belong to.

      -- ALGLIB --
         Copyright 21.03.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void kmeansgenerate(double[,] xy, int npoints, int nvars, int k, int restarts, out int info, out double[,] c, out int[] xyc)
    {
        info = 0;
        c = new double[0,0];
        xyc = new int[0];
        kmeans.kmeansgenerate(xy, npoints, nvars, k, restarts, ref info, ref c, ref xyc);
        return;
    }

}
public partial class alglib
{


    /*************************************************************************
    Multiclass Fisher LDA

    Subroutine finds coefficients of linear combination which optimally separates
    training set on classes.

    INPUT PARAMETERS:
        XY          -   training set, array[0..NPoints-1,0..NVars].
                        First NVars columns store values of independent
                        variables, next column stores number of class (from 0
                        to NClasses-1) which dataset element belongs to. Fractional
                        values are rounded to nearest integer.
        NPoints     -   training set size, NPoints>=0
        NVars       -   number of independent variables, NVars>=1
        NClasses    -   number of classes, NClasses>=2


    OUTPUT PARAMETERS:
        Info        -   return code:
                        * -4, if internal EVD subroutine hasn't converged
                        * -2, if there is a point with class number
                              outside of [0..NClasses-1].
                        * -1, if incorrect parameters was passed (NPoints<0,
                              NVars<1, NClasses<2)
                        *  1, if task has been solved
                        *  2, if there was a multicollinearity in training set,
                              but task has been solved.
        W           -   linear combination coefficients, array[0..NVars-1]

      -- ALGLIB --
         Copyright 31.05.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void fisherlda(double[,] xy, int npoints, int nvars, int nclasses, out int info, out double[] w)
    {
        info = 0;
        w = new double[0];
        lda.fisherlda(xy, npoints, nvars, nclasses, ref info, ref w);
        return;
    }

    /*************************************************************************
    N-dimensional multiclass Fisher LDA

    Subroutine finds coefficients of linear combinations which optimally separates
    training set on classes. It returns N-dimensional basis whose vector are sorted
    by quality of training set separation (in descending order).

    INPUT PARAMETERS:
        XY          -   training set, array[0..NPoints-1,0..NVars].
                        First NVars columns store values of independent
                        variables, next column stores number of class (from 0
                        to NClasses-1) which dataset element belongs to. Fractional
                        values are rounded to nearest integer.
        NPoints     -   training set size, NPoints>=0
        NVars       -   number of independent variables, NVars>=1
        NClasses    -   number of classes, NClasses>=2


    OUTPUT PARAMETERS:
        Info        -   return code:
                        * -4, if internal EVD subroutine hasn't converged
                        * -2, if there is a point with class number
                              outside of [0..NClasses-1].
                        * -1, if incorrect parameters was passed (NPoints<0,
                              NVars<1, NClasses<2)
                        *  1, if task has been solved
                        *  2, if there was a multicollinearity in training set,
                              but task has been solved.
        W           -   basis, array[0..NVars-1,0..NVars-1]
                        columns of matrix stores basis vectors, sorted by
                        quality of training set separation (in descending order)

      -- ALGLIB --
         Copyright 31.05.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void fisherldan(double[,] xy, int npoints, int nvars, int nclasses, out int info, out double[,] w)
    {
        info = 0;
        w = new double[0,0];
        lda.fisherldan(xy, npoints, nvars, nclasses, ref info, ref w);
        return;
    }

}
public partial class alglib
{


    /*************************************************************************

    *************************************************************************/
    public class linearmodel
    {
        //
        // Public declarations
        //

        public linearmodel()
        {
            _innerobj = new linreg.linearmodel();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private linreg.linearmodel _innerobj;
        public linreg.linearmodel innerobj { get { return _innerobj; } }
        public linearmodel(linreg.linearmodel obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************
    LRReport structure contains additional information about linear model:
    * C             -   covariation matrix,  array[0..NVars,0..NVars].
                        C[i,j] = Cov(A[i],A[j])
    * RMSError      -   root mean square error on a training set
    * AvgError      -   average error on a training set
    * AvgRelError   -   average relative error on a training set (excluding
                        observations with zero function value).
    * CVRMSError    -   leave-one-out cross-validation estimate of
                        generalization error. Calculated using fast algorithm
                        with O(NVars*NPoints) complexity.
    * CVAvgError    -   cross-validation estimate of average error
    * CVAvgRelError -   cross-validation estimate of average relative error

    All other fields of the structure are intended for internal use and should
    not be used outside ALGLIB.
    *************************************************************************/
    public class lrreport
    {
        //
        // Public declarations
        //
        public double[,] c { get { return _innerobj.c; } set { _innerobj.c = value; } }
        public double rmserror { get { return _innerobj.rmserror; } set { _innerobj.rmserror = value; } }
        public double avgerror { get { return _innerobj.avgerror; } set { _innerobj.avgerror = value; } }
        public double avgrelerror { get { return _innerobj.avgrelerror; } set { _innerobj.avgrelerror = value; } }
        public double cvrmserror { get { return _innerobj.cvrmserror; } set { _innerobj.cvrmserror = value; } }
        public double cvavgerror { get { return _innerobj.cvavgerror; } set { _innerobj.cvavgerror = value; } }
        public double cvavgrelerror { get { return _innerobj.cvavgrelerror; } set { _innerobj.cvavgrelerror = value; } }
        public int ncvdefects { get { return _innerobj.ncvdefects; } set { _innerobj.ncvdefects = value; } }
        public int[] cvdefects { get { return _innerobj.cvdefects; } set { _innerobj.cvdefects = value; } }

        public lrreport()
        {
            _innerobj = new linreg.lrreport();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private linreg.lrreport _innerobj;
        public linreg.lrreport innerobj { get { return _innerobj; } }
        public lrreport(linreg.lrreport obj)
        {
            _innerobj = obj;
        }
    }

    /*************************************************************************
    Linear regression

    Subroutine builds model:

        Y = A(0)*X[0] + ... + A(N-1)*X[N-1] + A(N)

    and model found in ALGLIB format, covariation matrix, training set  errors
    (rms,  average,  average  relative)   and  leave-one-out  cross-validation
    estimate of the generalization error. CV  estimate calculated  using  fast
    algorithm with O(NPoints*NVars) complexity.

    When  covariation  matrix  is  calculated  standard deviations of function
    values are assumed to be equal to RMS error on the training set.

    INPUT PARAMETERS:
        XY          -   training set, array [0..NPoints-1,0..NVars]:
                        * NVars columns - independent variables
                        * last column - dependent variable
        NPoints     -   training set size, NPoints>NVars+1
        NVars       -   number of independent variables

    OUTPUT PARAMETERS:
        Info        -   return code:
                        * -255, in case of unknown internal error
                        * -4, if internal SVD subroutine haven't converged
                        * -1, if incorrect parameters was passed (NPoints<NVars+2, NVars<1).
                        *  1, if subroutine successfully finished
        LM          -   linear model in the ALGLIB format. Use subroutines of
                        this unit to work with the model.
        AR          -   additional results


      -- ALGLIB --
         Copyright 02.08.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void lrbuild(double[,] xy, int npoints, int nvars, out int info, out linearmodel lm, out lrreport ar)
    {
        info = 0;
        lm = new linearmodel();
        ar = new lrreport();
        linreg.lrbuild(xy, npoints, nvars, ref info, lm.innerobj, ar.innerobj);
        return;
    }

    /*************************************************************************
    Linear regression

    Variant of LRBuild which uses vector of standatd deviations (errors in
    function values).

    INPUT PARAMETERS:
        XY          -   training set, array [0..NPoints-1,0..NVars]:
                        * NVars columns - independent variables
                        * last column - dependent variable
        S           -   standard deviations (errors in function values)
                        array[0..NPoints-1], S[i]>0.
        NPoints     -   training set size, NPoints>NVars+1
        NVars       -   number of independent variables

    OUTPUT PARAMETERS:
        Info        -   return code:
                        * -255, in case of unknown internal error
                        * -4, if internal SVD subroutine haven't converged
                        * -1, if incorrect parameters was passed (NPoints<NVars+2, NVars<1).
                        * -2, if S[I]<=0
                        *  1, if subroutine successfully finished
        LM          -   linear model in the ALGLIB format. Use subroutines of
                        this unit to work with the model.
        AR          -   additional results


      -- ALGLIB --
         Copyright 02.08.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void lrbuilds(double[,] xy, double[] s, int npoints, int nvars, out int info, out linearmodel lm, out lrreport ar)
    {
        info = 0;
        lm = new linearmodel();
        ar = new lrreport();
        linreg.lrbuilds(xy, s, npoints, nvars, ref info, lm.innerobj, ar.innerobj);
        return;
    }

    /*************************************************************************
    Like LRBuildS, but builds model

        Y = A(0)*X[0] + ... + A(N-1)*X[N-1]

    i.e. with zero constant term.

      -- ALGLIB --
         Copyright 30.10.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void lrbuildzs(double[,] xy, double[] s, int npoints, int nvars, out int info, out linearmodel lm, out lrreport ar)
    {
        info = 0;
        lm = new linearmodel();
        ar = new lrreport();
        linreg.lrbuildzs(xy, s, npoints, nvars, ref info, lm.innerobj, ar.innerobj);
        return;
    }

    /*************************************************************************
    Like LRBuild but builds model

        Y = A(0)*X[0] + ... + A(N-1)*X[N-1]

    i.e. with zero constant term.

      -- ALGLIB --
         Copyright 30.10.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void lrbuildz(double[,] xy, int npoints, int nvars, out int info, out linearmodel lm, out lrreport ar)
    {
        info = 0;
        lm = new linearmodel();
        ar = new lrreport();
        linreg.lrbuildz(xy, npoints, nvars, ref info, lm.innerobj, ar.innerobj);
        return;
    }

    /*************************************************************************
    Unpacks coefficients of linear model.

    INPUT PARAMETERS:
        LM          -   linear model in ALGLIB format

    OUTPUT PARAMETERS:
        V           -   coefficients, array[0..NVars]
                        constant term (intercept) is stored in the V[NVars].
        NVars       -   number of independent variables (one less than number
                        of coefficients)

      -- ALGLIB --
         Copyright 30.08.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void lrunpack(linearmodel lm, out double[] v, out int nvars)
    {
        v = new double[0];
        nvars = 0;
        linreg.lrunpack(lm.innerobj, ref v, ref nvars);
        return;
    }

    /*************************************************************************
    "Packs" coefficients and creates linear model in ALGLIB format (LRUnpack
    reversed).

    INPUT PARAMETERS:
        V           -   coefficients, array[0..NVars]
        NVars       -   number of independent variables

    OUTPUT PAREMETERS:
        LM          -   linear model.

      -- ALGLIB --
         Copyright 30.08.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void lrpack(double[] v, int nvars, out linearmodel lm)
    {
        lm = new linearmodel();
        linreg.lrpack(v, nvars, lm.innerobj);
        return;
    }

    /*************************************************************************
    Procesing

    INPUT PARAMETERS:
        LM      -   linear model
        X       -   input vector,  array[0..NVars-1].

    Result:
        value of linear model regression estimate

      -- ALGLIB --
         Copyright 03.09.2008 by Bochkanov Sergey
    *************************************************************************/
    public static double lrprocess(linearmodel lm, double[] x)
    {

        double result = linreg.lrprocess(lm.innerobj, x);
        return result;
    }

    /*************************************************************************
    RMS error on the test set

    INPUT PARAMETERS:
        LM      -   linear model
        XY      -   test set
        NPoints -   test set size

    RESULT:
        root mean square error.

      -- ALGLIB --
         Copyright 30.08.2008 by Bochkanov Sergey
    *************************************************************************/
    public static double lrrmserror(linearmodel lm, double[,] xy, int npoints)
    {

        double result = linreg.lrrmserror(lm.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    Average error on the test set

    INPUT PARAMETERS:
        LM      -   linear model
        XY      -   test set
        NPoints -   test set size

    RESULT:
        average error.

      -- ALGLIB --
         Copyright 30.08.2008 by Bochkanov Sergey
    *************************************************************************/
    public static double lravgerror(linearmodel lm, double[,] xy, int npoints)
    {

        double result = linreg.lravgerror(lm.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    RMS error on the test set

    INPUT PARAMETERS:
        LM      -   linear model
        XY      -   test set
        NPoints -   test set size

    RESULT:
        average relative error.

      -- ALGLIB --
         Copyright 30.08.2008 by Bochkanov Sergey
    *************************************************************************/
    public static double lravgrelerror(linearmodel lm, double[,] xy, int npoints)
    {

        double result = linreg.lravgrelerror(lm.innerobj, xy, npoints);
        return result;
    }

}
public partial class alglib
{


    /*************************************************************************

    *************************************************************************/
    public class multilayerperceptron
    {
        //
        // Public declarations
        //

        public multilayerperceptron()
        {
            _innerobj = new mlpbase.multilayerperceptron();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private mlpbase.multilayerperceptron _innerobj;
        public mlpbase.multilayerperceptron innerobj { get { return _innerobj; } }
        public multilayerperceptron(mlpbase.multilayerperceptron obj)
        {
            _innerobj = obj;
        }
    }

    /*************************************************************************
    Creates  neural  network  with  NIn  inputs,  NOut outputs, without hidden
    layers, with linear output layer. Network weights are  filled  with  small
    random values.

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpcreate0(int nin, int nout, out multilayerperceptron network)
    {
        network = new multilayerperceptron();
        mlpbase.mlpcreate0(nin, nout, network.innerobj);
        return;
    }

    /*************************************************************************
    Same  as  MLPCreate0,  but  with  one  hidden  layer  (NHid  neurons) with
    non-linear activation function. Output layer is linear.

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpcreate1(int nin, int nhid, int nout, out multilayerperceptron network)
    {
        network = new multilayerperceptron();
        mlpbase.mlpcreate1(nin, nhid, nout, network.innerobj);
        return;
    }

    /*************************************************************************
    Same as MLPCreate0, but with two hidden layers (NHid1 and  NHid2  neurons)
    with non-linear activation function. Output layer is linear.
     $ALL

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpcreate2(int nin, int nhid1, int nhid2, int nout, out multilayerperceptron network)
    {
        network = new multilayerperceptron();
        mlpbase.mlpcreate2(nin, nhid1, nhid2, nout, network.innerobj);
        return;
    }

    /*************************************************************************
    Creates  neural  network  with  NIn  inputs,  NOut outputs, without hidden
    layers with non-linear output layer. Network weights are filled with small
    random values.

    Activation function of the output layer takes values:

        (B, +INF), if D>=0

    or

        (-INF, B), if D<0.


      -- ALGLIB --
         Copyright 30.03.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpcreateb0(int nin, int nout, double b, double d, out multilayerperceptron network)
    {
        network = new multilayerperceptron();
        mlpbase.mlpcreateb0(nin, nout, b, d, network.innerobj);
        return;
    }

    /*************************************************************************
    Same as MLPCreateB0 but with non-linear hidden layer.

      -- ALGLIB --
         Copyright 30.03.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpcreateb1(int nin, int nhid, int nout, double b, double d, out multilayerperceptron network)
    {
        network = new multilayerperceptron();
        mlpbase.mlpcreateb1(nin, nhid, nout, b, d, network.innerobj);
        return;
    }

    /*************************************************************************
    Same as MLPCreateB0 but with two non-linear hidden layers.

      -- ALGLIB --
         Copyright 30.03.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpcreateb2(int nin, int nhid1, int nhid2, int nout, double b, double d, out multilayerperceptron network)
    {
        network = new multilayerperceptron();
        mlpbase.mlpcreateb2(nin, nhid1, nhid2, nout, b, d, network.innerobj);
        return;
    }

    /*************************************************************************
    Creates  neural  network  with  NIn  inputs,  NOut outputs, without hidden
    layers with non-linear output layer. Network weights are filled with small
    random values. Activation function of the output layer takes values [A,B].

      -- ALGLIB --
         Copyright 30.03.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpcreater0(int nin, int nout, double a, double b, out multilayerperceptron network)
    {
        network = new multilayerperceptron();
        mlpbase.mlpcreater0(nin, nout, a, b, network.innerobj);
        return;
    }

    /*************************************************************************
    Same as MLPCreateR0, but with non-linear hidden layer.

      -- ALGLIB --
         Copyright 30.03.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpcreater1(int nin, int nhid, int nout, double a, double b, out multilayerperceptron network)
    {
        network = new multilayerperceptron();
        mlpbase.mlpcreater1(nin, nhid, nout, a, b, network.innerobj);
        return;
    }

    /*************************************************************************
    Same as MLPCreateR0, but with two non-linear hidden layers.

      -- ALGLIB --
         Copyright 30.03.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpcreater2(int nin, int nhid1, int nhid2, int nout, double a, double b, out multilayerperceptron network)
    {
        network = new multilayerperceptron();
        mlpbase.mlpcreater2(nin, nhid1, nhid2, nout, a, b, network.innerobj);
        return;
    }

    /*************************************************************************
    Creates classifier network with NIn  inputs  and  NOut  possible  classes.
    Network contains no hidden layers and linear output  layer  with  SOFTMAX-
    normalization  (so  outputs  sums  up  to  1.0  and  converge to posterior
    probabilities).

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpcreatec0(int nin, int nout, out multilayerperceptron network)
    {
        network = new multilayerperceptron();
        mlpbase.mlpcreatec0(nin, nout, network.innerobj);
        return;
    }

    /*************************************************************************
    Same as MLPCreateC0, but with one non-linear hidden layer.

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpcreatec1(int nin, int nhid, int nout, out multilayerperceptron network)
    {
        network = new multilayerperceptron();
        mlpbase.mlpcreatec1(nin, nhid, nout, network.innerobj);
        return;
    }

    /*************************************************************************
    Same as MLPCreateC0, but with two non-linear hidden layers.

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpcreatec2(int nin, int nhid1, int nhid2, int nout, out multilayerperceptron network)
    {
        network = new multilayerperceptron();
        mlpbase.mlpcreatec2(nin, nhid1, nhid2, nout, network.innerobj);
        return;
    }

    /*************************************************************************
    Randomization of neural network weights

      -- ALGLIB --
         Copyright 06.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlprandomize(multilayerperceptron network)
    {

        mlpbase.mlprandomize(network.innerobj);
        return;
    }

    /*************************************************************************
    Randomization of neural network weights and standartisator

      -- ALGLIB --
         Copyright 10.03.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void mlprandomizefull(multilayerperceptron network)
    {

        mlpbase.mlprandomizefull(network.innerobj);
        return;
    }

    /*************************************************************************
    Returns information about initialized network: number of inputs, outputs,
    weights.

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpproperties(multilayerperceptron network, out int nin, out int nout, out int wcount)
    {
        nin = 0;
        nout = 0;
        wcount = 0;
        mlpbase.mlpproperties(network.innerobj, ref nin, ref nout, ref wcount);
        return;
    }

    /*************************************************************************
    Tells whether network is SOFTMAX-normalized (i.e. classifier) or not.

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static bool mlpissoftmax(multilayerperceptron network)
    {

        bool result = mlpbase.mlpissoftmax(network.innerobj);
        return result;
    }

    /*************************************************************************
    Procesing

    INPUT PARAMETERS:
        Network -   neural network
        X       -   input vector,  array[0..NIn-1].

    OUTPUT PARAMETERS:
        Y       -   result. Regression estimate when solving regression  task,
                    vector of posterior probabilities for classification task.

    See also MLPProcessI

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpprocess(multilayerperceptron network, double[] x, ref double[] y)
    {

        mlpbase.mlpprocess(network.innerobj, x, ref y);
        return;
    }

    /*************************************************************************
    'interactive'  variant  of  MLPProcess  for  languages  like  Python which
    support constructs like "Y = MLPProcess(NN,X)" and interactive mode of the
    interpreter

    This function allocates new array on each call,  so  it  is  significantly
    slower than its 'non-interactive' counterpart, but it is  more  convenient
    when you call it from command line.

      -- ALGLIB --
         Copyright 21.09.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpprocessi(multilayerperceptron network, double[] x, out double[] y)
    {
        y = new double[0];
        mlpbase.mlpprocessi(network.innerobj, x, ref y);
        return;
    }

    /*************************************************************************
    Error function for neural network, internal subroutine.

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static double mlperror(multilayerperceptron network, double[,] xy, int ssize)
    {

        double result = mlpbase.mlperror(network.innerobj, xy, ssize);
        return result;
    }

    /*************************************************************************
    Natural error function for neural network, internal subroutine.

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static double mlperrorn(multilayerperceptron network, double[,] xy, int ssize)
    {

        double result = mlpbase.mlperrorn(network.innerobj, xy, ssize);
        return result;
    }

    /*************************************************************************
    Classification error

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static int mlpclserror(multilayerperceptron network, double[,] xy, int ssize)
    {

        int result = mlpbase.mlpclserror(network.innerobj, xy, ssize);
        return result;
    }

    /*************************************************************************
    Relative classification error on the test set

    INPUT PARAMETERS:
        Network -   network
        XY      -   test set
        NPoints -   test set size

    RESULT:
        percent of incorrectly classified cases. Works both for
        classifier networks and general purpose networks used as
        classifiers.

      -- ALGLIB --
         Copyright 25.12.2008 by Bochkanov Sergey
    *************************************************************************/
    public static double mlprelclserror(multilayerperceptron network, double[,] xy, int npoints)
    {

        double result = mlpbase.mlprelclserror(network.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    Average cross-entropy (in bits per element) on the test set

    INPUT PARAMETERS:
        Network -   neural network
        XY      -   test set
        NPoints -   test set size

    RESULT:
        CrossEntropy/(NPoints*LN(2)).
        Zero if network solves regression task.

      -- ALGLIB --
         Copyright 08.01.2009 by Bochkanov Sergey
    *************************************************************************/
    public static double mlpavgce(multilayerperceptron network, double[,] xy, int npoints)
    {

        double result = mlpbase.mlpavgce(network.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    RMS error on the test set

    INPUT PARAMETERS:
        Network -   neural network
        XY      -   test set
        NPoints -   test set size

    RESULT:
        root mean square error.
        Its meaning for regression task is obvious. As for
        classification task, RMS error means error when estimating posterior
        probabilities.

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static double mlprmserror(multilayerperceptron network, double[,] xy, int npoints)
    {

        double result = mlpbase.mlprmserror(network.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    Average error on the test set

    INPUT PARAMETERS:
        Network -   neural network
        XY      -   test set
        NPoints -   test set size

    RESULT:
        Its meaning for regression task is obvious. As for
        classification task, it means average error when estimating posterior
        probabilities.

      -- ALGLIB --
         Copyright 11.03.2008 by Bochkanov Sergey
    *************************************************************************/
    public static double mlpavgerror(multilayerperceptron network, double[,] xy, int npoints)
    {

        double result = mlpbase.mlpavgerror(network.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    Average relative error on the test set

    INPUT PARAMETERS:
        Network -   neural network
        XY      -   test set
        NPoints -   test set size

    RESULT:
        Its meaning for regression task is obvious. As for
        classification task, it means average relative error when estimating
        posterior probability of belonging to the correct class.

      -- ALGLIB --
         Copyright 11.03.2008 by Bochkanov Sergey
    *************************************************************************/
    public static double mlpavgrelerror(multilayerperceptron network, double[,] xy, int npoints)
    {

        double result = mlpbase.mlpavgrelerror(network.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    Gradient calculation

    INPUT PARAMETERS:
        Network -   network initialized with one of the network creation funcs
        X       -   input vector, length of array must be at least NIn
        DesiredY-   desired outputs, length of array must be at least NOut
        Grad    -   possibly preallocated array. If size of array is smaller
                    than WCount, it will be reallocated. It is recommended to
                    reuse previously allocated array to reduce allocation
                    overhead.

    OUTPUT PARAMETERS:
        E       -   error function, SUM(sqr(y[i]-desiredy[i])/2,i)
        Grad    -   gradient of E with respect to weights of network, array[WCount]

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpgrad(multilayerperceptron network, double[] x, double[] desiredy, out double e, ref double[] grad)
    {
        e = 0;
        mlpbase.mlpgrad(network.innerobj, x, desiredy, ref e, ref grad);
        return;
    }

    /*************************************************************************
    Gradient calculation (natural error function is used)

    INPUT PARAMETERS:
        Network -   network initialized with one of the network creation funcs
        X       -   input vector, length of array must be at least NIn
        DesiredY-   desired outputs, length of array must be at least NOut
        Grad    -   possibly preallocated array. If size of array is smaller
                    than WCount, it will be reallocated. It is recommended to
                    reuse previously allocated array to reduce allocation
                    overhead.

    OUTPUT PARAMETERS:
        E       -   error function, sum-of-squares for regression networks,
                    cross-entropy for classification networks.
        Grad    -   gradient of E with respect to weights of network, array[WCount]

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpgradn(multilayerperceptron network, double[] x, double[] desiredy, out double e, ref double[] grad)
    {
        e = 0;
        mlpbase.mlpgradn(network.innerobj, x, desiredy, ref e, ref grad);
        return;
    }

    /*************************************************************************
    Batch gradient calculation for a set of inputs/outputs

    INPUT PARAMETERS:
        Network -   network initialized with one of the network creation funcs
        XY      -   set of inputs/outputs; one sample = one row;
                    first NIn columns contain inputs,
                    next NOut columns - desired outputs.
        SSize   -   number of elements in XY
        Grad    -   possibly preallocated array. If size of array is smaller
                    than WCount, it will be reallocated. It is recommended to
                    reuse previously allocated array to reduce allocation
                    overhead.

    OUTPUT PARAMETERS:
        E       -   error function, SUM(sqr(y[i]-desiredy[i])/2,i)
        Grad    -   gradient of E with respect to weights of network, array[WCount]

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpgradbatch(multilayerperceptron network, double[,] xy, int ssize, out double e, ref double[] grad)
    {
        e = 0;
        mlpbase.mlpgradbatch(network.innerobj, xy, ssize, ref e, ref grad);
        return;
    }

    /*************************************************************************
    Batch gradient calculation for a set of inputs/outputs
    (natural error function is used)

    INPUT PARAMETERS:
        Network -   network initialized with one of the network creation funcs
        XY      -   set of inputs/outputs; one sample = one row;
                    first NIn columns contain inputs,
                    next NOut columns - desired outputs.
        SSize   -   number of elements in XY
        Grad    -   possibly preallocated array. If size of array is smaller
                    than WCount, it will be reallocated. It is recommended to
                    reuse previously allocated array to reduce allocation
                    overhead.

    OUTPUT PARAMETERS:
        E       -   error function, sum-of-squares for regression networks,
                    cross-entropy for classification networks.
        Grad    -   gradient of E with respect to weights of network, array[WCount]

      -- ALGLIB --
         Copyright 04.11.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpgradnbatch(multilayerperceptron network, double[,] xy, int ssize, out double e, ref double[] grad)
    {
        e = 0;
        mlpbase.mlpgradnbatch(network.innerobj, xy, ssize, ref e, ref grad);
        return;
    }

    /*************************************************************************
    Batch Hessian calculation (natural error function) using R-algorithm.
    Internal subroutine.

      -- ALGLIB --
         Copyright 26.01.2008 by Bochkanov Sergey.

         Hessian calculation based on R-algorithm described in
         "Fast Exact Multiplication by the Hessian",
         B. A. Pearlmutter,
         Neural Computation, 1994.
    *************************************************************************/
    public static void mlphessiannbatch(multilayerperceptron network, double[,] xy, int ssize, out double e, ref double[] grad, ref double[,] h)
    {
        e = 0;
        mlpbase.mlphessiannbatch(network.innerobj, xy, ssize, ref e, ref grad, ref h);
        return;
    }

    /*************************************************************************
    Batch Hessian calculation using R-algorithm.
    Internal subroutine.

      -- ALGLIB --
         Copyright 26.01.2008 by Bochkanov Sergey.

         Hessian calculation based on R-algorithm described in
         "Fast Exact Multiplication by the Hessian",
         B. A. Pearlmutter,
         Neural Computation, 1994.
    *************************************************************************/
    public static void mlphessianbatch(multilayerperceptron network, double[,] xy, int ssize, out double e, ref double[] grad, ref double[,] h)
    {
        e = 0;
        mlpbase.mlphessianbatch(network.innerobj, xy, ssize, ref e, ref grad, ref h);
        return;
    }

}
public partial class alglib
{


    /*************************************************************************

    *************************************************************************/
    public class logitmodel
    {
        //
        // Public declarations
        //

        public logitmodel()
        {
            _innerobj = new logit.logitmodel();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private logit.logitmodel _innerobj;
        public logit.logitmodel innerobj { get { return _innerobj; } }
        public logitmodel(logit.logitmodel obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************
    MNLReport structure contains information about training process:
    * NGrad     -   number of gradient calculations
    * NHess     -   number of Hessian calculations
    *************************************************************************/
    public class mnlreport
    {
        //
        // Public declarations
        //
        public int ngrad { get { return _innerobj.ngrad; } set { _innerobj.ngrad = value; } }
        public int nhess { get { return _innerobj.nhess; } set { _innerobj.nhess = value; } }

        public mnlreport()
        {
            _innerobj = new logit.mnlreport();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private logit.mnlreport _innerobj;
        public logit.mnlreport innerobj { get { return _innerobj; } }
        public mnlreport(logit.mnlreport obj)
        {
            _innerobj = obj;
        }
    }

    /*************************************************************************
    This subroutine trains logit model.

    INPUT PARAMETERS:
        XY          -   training set, array[0..NPoints-1,0..NVars]
                        First NVars columns store values of independent
                        variables, next column stores number of class (from 0
                        to NClasses-1) which dataset element belongs to. Fractional
                        values are rounded to nearest integer.
        NPoints     -   training set size, NPoints>=1
        NVars       -   number of independent variables, NVars>=1
        NClasses    -   number of classes, NClasses>=2

    OUTPUT PARAMETERS:
        Info        -   return code:
                        * -2, if there is a point with class number
                              outside of [0..NClasses-1].
                        * -1, if incorrect parameters was passed
                              (NPoints<NVars+2, NVars<1, NClasses<2).
                        *  1, if task has been solved
        LM          -   model built
        Rep         -   training report

      -- ALGLIB --
         Copyright 10.09.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void mnltrainh(double[,] xy, int npoints, int nvars, int nclasses, out int info, out logitmodel lm, out mnlreport rep)
    {
        info = 0;
        lm = new logitmodel();
        rep = new mnlreport();
        logit.mnltrainh(xy, npoints, nvars, nclasses, ref info, lm.innerobj, rep.innerobj);
        return;
    }

    /*************************************************************************
    Procesing

    INPUT PARAMETERS:
        LM      -   logit model, passed by non-constant reference
                    (some fields of structure are used as temporaries
                    when calculating model output).
        X       -   input vector,  array[0..NVars-1].
        Y       -   (possibly) preallocated buffer; if size of Y is less than
                    NClasses, it will be reallocated.If it is large enough, it
                    is NOT reallocated, so we can save some time on reallocation.

    OUTPUT PARAMETERS:
        Y       -   result, array[0..NClasses-1]
                    Vector of posterior probabilities for classification task.

      -- ALGLIB --
         Copyright 10.09.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void mnlprocess(logitmodel lm, double[] x, ref double[] y)
    {

        logit.mnlprocess(lm.innerobj, x, ref y);
        return;
    }

    /*************************************************************************
    'interactive'  variant  of  MNLProcess  for  languages  like  Python which
    support constructs like "Y = MNLProcess(LM,X)" and interactive mode of the
    interpreter

    This function allocates new array on each call,  so  it  is  significantly
    slower than its 'non-interactive' counterpart, but it is  more  convenient
    when you call it from command line.

      -- ALGLIB --
         Copyright 10.09.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void mnlprocessi(logitmodel lm, double[] x, out double[] y)
    {
        y = new double[0];
        logit.mnlprocessi(lm.innerobj, x, ref y);
        return;
    }

    /*************************************************************************
    Unpacks coefficients of logit model. Logit model have form:

        P(class=i) = S(i) / (S(0) + S(1) + ... +S(M-1))
              S(i) = Exp(A[i,0]*X[0] + ... + A[i,N-1]*X[N-1] + A[i,N]), when i<M-1
            S(M-1) = 1

    INPUT PARAMETERS:
        LM          -   logit model in ALGLIB format

    OUTPUT PARAMETERS:
        V           -   coefficients, array[0..NClasses-2,0..NVars]
        NVars       -   number of independent variables
        NClasses    -   number of classes

      -- ALGLIB --
         Copyright 10.09.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void mnlunpack(logitmodel lm, out double[,] a, out int nvars, out int nclasses)
    {
        a = new double[0,0];
        nvars = 0;
        nclasses = 0;
        logit.mnlunpack(lm.innerobj, ref a, ref nvars, ref nclasses);
        return;
    }

    /*************************************************************************
    "Packs" coefficients and creates logit model in ALGLIB format (MNLUnpack
    reversed).

    INPUT PARAMETERS:
        A           -   model (see MNLUnpack)
        NVars       -   number of independent variables
        NClasses    -   number of classes

    OUTPUT PARAMETERS:
        LM          -   logit model.

      -- ALGLIB --
         Copyright 10.09.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void mnlpack(double[,] a, int nvars, int nclasses, out logitmodel lm)
    {
        lm = new logitmodel();
        logit.mnlpack(a, nvars, nclasses, lm.innerobj);
        return;
    }

    /*************************************************************************
    Average cross-entropy (in bits per element) on the test set

    INPUT PARAMETERS:
        LM      -   logit model
        XY      -   test set
        NPoints -   test set size

    RESULT:
        CrossEntropy/(NPoints*ln(2)).

      -- ALGLIB --
         Copyright 10.09.2008 by Bochkanov Sergey
    *************************************************************************/
    public static double mnlavgce(logitmodel lm, double[,] xy, int npoints)
    {

        double result = logit.mnlavgce(lm.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    Relative classification error on the test set

    INPUT PARAMETERS:
        LM      -   logit model
        XY      -   test set
        NPoints -   test set size

    RESULT:
        percent of incorrectly classified cases.

      -- ALGLIB --
         Copyright 10.09.2008 by Bochkanov Sergey
    *************************************************************************/
    public static double mnlrelclserror(logitmodel lm, double[,] xy, int npoints)
    {

        double result = logit.mnlrelclserror(lm.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    RMS error on the test set

    INPUT PARAMETERS:
        LM      -   logit model
        XY      -   test set
        NPoints -   test set size

    RESULT:
        root mean square error (error when estimating posterior probabilities).

      -- ALGLIB --
         Copyright 30.08.2008 by Bochkanov Sergey
    *************************************************************************/
    public static double mnlrmserror(logitmodel lm, double[,] xy, int npoints)
    {

        double result = logit.mnlrmserror(lm.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    Average error on the test set

    INPUT PARAMETERS:
        LM      -   logit model
        XY      -   test set
        NPoints -   test set size

    RESULT:
        average error (error when estimating posterior probabilities).

      -- ALGLIB --
         Copyright 30.08.2008 by Bochkanov Sergey
    *************************************************************************/
    public static double mnlavgerror(logitmodel lm, double[,] xy, int npoints)
    {

        double result = logit.mnlavgerror(lm.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    Average relative error on the test set

    INPUT PARAMETERS:
        LM      -   logit model
        XY      -   test set
        NPoints -   test set size

    RESULT:
        average relative error (error when estimating posterior probabilities).

      -- ALGLIB --
         Copyright 30.08.2008 by Bochkanov Sergey
    *************************************************************************/
    public static double mnlavgrelerror(logitmodel lm, double[,] xy, int ssize)
    {

        double result = logit.mnlavgrelerror(lm.innerobj, xy, ssize);
        return result;
    }

    /*************************************************************************
    Classification error on test set = MNLRelClsError*NPoints

      -- ALGLIB --
         Copyright 10.09.2008 by Bochkanov Sergey
    *************************************************************************/
    public static int mnlclserror(logitmodel lm, double[,] xy, int npoints)
    {

        int result = logit.mnlclserror(lm.innerobj, xy, npoints);
        return result;
    }

}
public partial class alglib
{


    /*************************************************************************
    Training report:
        * NGrad     - number of gradient calculations
        * NHess     - number of Hessian calculations
        * NCholesky - number of Cholesky decompositions
    *************************************************************************/
    public class mlpreport
    {
        //
        // Public declarations
        //
        public int ngrad { get { return _innerobj.ngrad; } set { _innerobj.ngrad = value; } }
        public int nhess { get { return _innerobj.nhess; } set { _innerobj.nhess = value; } }
        public int ncholesky { get { return _innerobj.ncholesky; } set { _innerobj.ncholesky = value; } }

        public mlpreport()
        {
            _innerobj = new mlptrain.mlpreport();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private mlptrain.mlpreport _innerobj;
        public mlptrain.mlpreport innerobj { get { return _innerobj; } }
        public mlpreport(mlptrain.mlpreport obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************
    Cross-validation estimates of generalization error
    *************************************************************************/
    public class mlpcvreport
    {
        //
        // Public declarations
        //
        public double relclserror { get { return _innerobj.relclserror; } set { _innerobj.relclserror = value; } }
        public double avgce { get { return _innerobj.avgce; } set { _innerobj.avgce = value; } }
        public double rmserror { get { return _innerobj.rmserror; } set { _innerobj.rmserror = value; } }
        public double avgerror { get { return _innerobj.avgerror; } set { _innerobj.avgerror = value; } }
        public double avgrelerror { get { return _innerobj.avgrelerror; } set { _innerobj.avgrelerror = value; } }

        public mlpcvreport()
        {
            _innerobj = new mlptrain.mlpcvreport();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private mlptrain.mlpcvreport _innerobj;
        public mlptrain.mlpcvreport innerobj { get { return _innerobj; } }
        public mlpcvreport(mlptrain.mlpcvreport obj)
        {
            _innerobj = obj;
        }
    }

    /*************************************************************************
    Neural network training  using  modified  Levenberg-Marquardt  with  exact
    Hessian calculation and regularization. Subroutine trains  neural  network
    with restarts from random positions. Algorithm is well  suited  for  small
    and medium scale problems (hundreds of weights).

    INPUT PARAMETERS:
        Network     -   neural network with initialized geometry
        XY          -   training set
        NPoints     -   training set size
        Decay       -   weight decay constant, >=0.001
                        Decay term 'Decay*||Weights||^2' is added to error
                        function.
                        If you don't know what Decay to choose, use 0.001.
        Restarts    -   number of restarts from random position, >0.
                        If you don't know what Restarts to choose, use 2.

    OUTPUT PARAMETERS:
        Network     -   trained neural network.
        Info        -   return code:
                        * -9, if internal matrix inverse subroutine failed
                        * -2, if there is a point with class number
                              outside of [0..NOut-1].
                        * -1, if wrong parameters specified
                              (NPoints<0, Restarts<1).
                        *  2, if task has been solved.
        Rep         -   training report

      -- ALGLIB --
         Copyright 10.03.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlptrainlm(multilayerperceptron network, double[,] xy, int npoints, double decay, int restarts, out int info, out mlpreport rep)
    {
        info = 0;
        rep = new mlpreport();
        mlptrain.mlptrainlm(network.innerobj, xy, npoints, decay, restarts, ref info, rep.innerobj);
        return;
    }

    /*************************************************************************
    Neural  network  training  using  L-BFGS  algorithm  with  regularization.
    Subroutine  trains  neural  network  with  restarts from random positions.
    Algorithm  is  well  suited  for  problems  of  any dimensionality (memory
    requirements and step complexity are linear by weights number).

    INPUT PARAMETERS:
        Network     -   neural network with initialized geometry
        XY          -   training set
        NPoints     -   training set size
        Decay       -   weight decay constant, >=0.001
                        Decay term 'Decay*||Weights||^2' is added to error
                        function.
                        If you don't know what Decay to choose, use 0.001.
        Restarts    -   number of restarts from random position, >0.
                        If you don't know what Restarts to choose, use 2.
        WStep       -   stopping criterion. Algorithm stops if  step  size  is
                        less than WStep. Recommended value - 0.01.  Zero  step
                        size means stopping after MaxIts iterations.
        MaxIts      -   stopping   criterion.  Algorithm  stops  after  MaxIts
                        iterations (NOT gradient  calculations).  Zero  MaxIts
                        means stopping when step is sufficiently small.

    OUTPUT PARAMETERS:
        Network     -   trained neural network.
        Info        -   return code:
                        * -8, if both WStep=0 and MaxIts=0
                        * -2, if there is a point with class number
                              outside of [0..NOut-1].
                        * -1, if wrong parameters specified
                              (NPoints<0, Restarts<1).
                        *  2, if task has been solved.
        Rep         -   training report

      -- ALGLIB --
         Copyright 09.12.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlptrainlbfgs(multilayerperceptron network, double[,] xy, int npoints, double decay, int restarts, double wstep, int maxits, out int info, out mlpreport rep)
    {
        info = 0;
        rep = new mlpreport();
        mlptrain.mlptrainlbfgs(network.innerobj, xy, npoints, decay, restarts, wstep, maxits, ref info, rep.innerobj);
        return;
    }

    /*************************************************************************
    Neural network training using early stopping (base algorithm - L-BFGS with
    regularization).

    INPUT PARAMETERS:
        Network     -   neural network with initialized geometry
        TrnXY       -   training set
        TrnSize     -   training set size
        ValXY       -   validation set
        ValSize     -   validation set size
        Decay       -   weight decay constant, >=0.001
                        Decay term 'Decay*||Weights||^2' is added to error
                        function.
                        If you don't know what Decay to choose, use 0.001.
        Restarts    -   number of restarts from random position, >0.
                        If you don't know what Restarts to choose, use 2.

    OUTPUT PARAMETERS:
        Network     -   trained neural network.
        Info        -   return code:
                        * -2, if there is a point with class number
                              outside of [0..NOut-1].
                        * -1, if wrong parameters specified
                              (NPoints<0, Restarts<1, ...).
                        *  2, task has been solved, stopping  criterion  met -
                              sufficiently small step size.  Not expected  (we
                              use  EARLY  stopping)  but  possible  and not an
                              error.
                        *  6, task has been solved, stopping  criterion  met -
                              increasing of validation set error.
        Rep         -   training report

    NOTE:

    Algorithm stops if validation set error increases for  a  long  enough  or
    step size is small enought  (there  are  task  where  validation  set  may
    decrease for eternity). In any case solution returned corresponds  to  the
    minimum of validation set error.

      -- ALGLIB --
         Copyright 10.03.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlptraines(multilayerperceptron network, double[,] trnxy, int trnsize, double[,] valxy, int valsize, double decay, int restarts, out int info, out mlpreport rep)
    {
        info = 0;
        rep = new mlpreport();
        mlptrain.mlptraines(network.innerobj, trnxy, trnsize, valxy, valsize, decay, restarts, ref info, rep.innerobj);
        return;
    }

    /*************************************************************************
    Cross-validation estimate of generalization error.

    Base algorithm - L-BFGS.

    INPUT PARAMETERS:
        Network     -   neural network with initialized geometry.   Network is
                        not changed during cross-validation -  it is used only
                        as a representative of its architecture.
        XY          -   training set.
        SSize       -   training set size
        Decay       -   weight  decay, same as in MLPTrainLBFGS
        Restarts    -   number of restarts, >0.
                        restarts are counted for each partition separately, so
                        total number of restarts will be Restarts*FoldsCount.
        WStep       -   stopping criterion, same as in MLPTrainLBFGS
        MaxIts      -   stopping criterion, same as in MLPTrainLBFGS
        FoldsCount  -   number of folds in k-fold cross-validation,
                        2<=FoldsCount<=SSize.
                        recommended value: 10.

    OUTPUT PARAMETERS:
        Info        -   return code, same as in MLPTrainLBFGS
        Rep         -   report, same as in MLPTrainLM/MLPTrainLBFGS
        CVRep       -   generalization error estimates

      -- ALGLIB --
         Copyright 09.12.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpkfoldcvlbfgs(multilayerperceptron network, double[,] xy, int npoints, double decay, int restarts, double wstep, int maxits, int foldscount, out int info, out mlpreport rep, out mlpcvreport cvrep)
    {
        info = 0;
        rep = new mlpreport();
        cvrep = new mlpcvreport();
        mlptrain.mlpkfoldcvlbfgs(network.innerobj, xy, npoints, decay, restarts, wstep, maxits, foldscount, ref info, rep.innerobj, cvrep.innerobj);
        return;
    }

    /*************************************************************************
    Cross-validation estimate of generalization error.

    Base algorithm - Levenberg-Marquardt.

    INPUT PARAMETERS:
        Network     -   neural network with initialized geometry.   Network is
                        not changed during cross-validation -  it is used only
                        as a representative of its architecture.
        XY          -   training set.
        SSize       -   training set size
        Decay       -   weight  decay, same as in MLPTrainLBFGS
        Restarts    -   number of restarts, >0.
                        restarts are counted for each partition separately, so
                        total number of restarts will be Restarts*FoldsCount.
        FoldsCount  -   number of folds in k-fold cross-validation,
                        2<=FoldsCount<=SSize.
                        recommended value: 10.

    OUTPUT PARAMETERS:
        Info        -   return code, same as in MLPTrainLBFGS
        Rep         -   report, same as in MLPTrainLM/MLPTrainLBFGS
        CVRep       -   generalization error estimates

      -- ALGLIB --
         Copyright 09.12.2007 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpkfoldcvlm(multilayerperceptron network, double[,] xy, int npoints, double decay, int restarts, int foldscount, out int info, out mlpreport rep, out mlpcvreport cvrep)
    {
        info = 0;
        rep = new mlpreport();
        cvrep = new mlpcvreport();
        mlptrain.mlpkfoldcvlm(network.innerobj, xy, npoints, decay, restarts, foldscount, ref info, rep.innerobj, cvrep.innerobj);
        return;
    }

}
public partial class alglib
{


    /*************************************************************************
    Neural networks ensemble
    *************************************************************************/
    public class mlpensemble
    {
        //
        // Public declarations
        //

        public mlpensemble()
        {
            _innerobj = new mlpe.mlpensemble();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private mlpe.mlpensemble _innerobj;
        public mlpe.mlpensemble innerobj { get { return _innerobj; } }
        public mlpensemble(mlpe.mlpensemble obj)
        {
            _innerobj = obj;
        }
    }

    /*************************************************************************
    Like MLPCreate0, but for ensembles.

      -- ALGLIB --
         Copyright 18.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void n(int nin, int nout, int ensemblesize, out mlpensemble ensemble)
    {
        ensemble = new mlpensemble();
        mlpe.mlpecreate0(nin, nout, ensemblesize, ensemble.innerobj);
        return;
    }

    /*************************************************************************
    Like MLPCreate1, but for ensembles.

      -- ALGLIB --
         Copyright 18.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpecreate1(int nin, int nhid, int nout, int ensemblesize, out mlpensemble ensemble)
    {
        ensemble = new mlpensemble();
        mlpe.mlpecreate1(nin, nhid, nout, ensemblesize, ensemble.innerobj);
        return;
    }

    /*************************************************************************
    Like MLPCreate2, but for ensembles.

      -- ALGLIB --
         Copyright 18.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpecreate2(int nin, int nhid1, int nhid2, int nout, int ensemblesize, out mlpensemble ensemble)
    {
        ensemble = new mlpensemble();
        mlpe.mlpecreate2(nin, nhid1, nhid2, nout, ensemblesize, ensemble.innerobj);
        return;
    }

    /*************************************************************************
    Like MLPCreateB0, but for ensembles.

      -- ALGLIB --
         Copyright 18.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpecreateb0(int nin, int nout, double b, double d, int ensemblesize, out mlpensemble ensemble)
    {
        ensemble = new mlpensemble();
        mlpe.mlpecreateb0(nin, nout, b, d, ensemblesize, ensemble.innerobj);
        return;
    }

    /*************************************************************************
    Like MLPCreateB1, but for ensembles.

      -- ALGLIB --
         Copyright 18.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpecreateb1(int nin, int nhid, int nout, double b, double d, int ensemblesize, out mlpensemble ensemble)
    {
        ensemble = new mlpensemble();
        mlpe.mlpecreateb1(nin, nhid, nout, b, d, ensemblesize, ensemble.innerobj);
        return;
    }

    /*************************************************************************
    Like MLPCreateB2, but for ensembles.

      -- ALGLIB --
         Copyright 18.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpecreateb2(int nin, int nhid1, int nhid2, int nout, double b, double d, int ensemblesize, out mlpensemble ensemble)
    {
        ensemble = new mlpensemble();
        mlpe.mlpecreateb2(nin, nhid1, nhid2, nout, b, d, ensemblesize, ensemble.innerobj);
        return;
    }

    /*************************************************************************
    Like MLPCreateR0, but for ensembles.

      -- ALGLIB --
         Copyright 18.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpecreater0(int nin, int nout, double a, double b, int ensemblesize, out mlpensemble ensemble)
    {
        ensemble = new mlpensemble();
        mlpe.mlpecreater0(nin, nout, a, b, ensemblesize, ensemble.innerobj);
        return;
    }

    /*************************************************************************
    Like MLPCreateR1, but for ensembles.

      -- ALGLIB --
         Copyright 18.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpecreater1(int nin, int nhid, int nout, double a, double b, int ensemblesize, out mlpensemble ensemble)
    {
        ensemble = new mlpensemble();
        mlpe.mlpecreater1(nin, nhid, nout, a, b, ensemblesize, ensemble.innerobj);
        return;
    }

    /*************************************************************************
    Like MLPCreateR2, but for ensembles.

      -- ALGLIB --
         Copyright 18.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpecreater2(int nin, int nhid1, int nhid2, int nout, double a, double b, int ensemblesize, out mlpensemble ensemble)
    {
        ensemble = new mlpensemble();
        mlpe.mlpecreater2(nin, nhid1, nhid2, nout, a, b, ensemblesize, ensemble.innerobj);
        return;
    }

    /*************************************************************************
    Like MLPCreateC0, but for ensembles.

      -- ALGLIB --
         Copyright 18.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpecreatec0(int nin, int nout, int ensemblesize, out mlpensemble ensemble)
    {
        ensemble = new mlpensemble();
        mlpe.mlpecreatec0(nin, nout, ensemblesize, ensemble.innerobj);
        return;
    }

    /*************************************************************************
    Like MLPCreateC1, but for ensembles.

      -- ALGLIB --
         Copyright 18.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpecreatec1(int nin, int nhid, int nout, int ensemblesize, out mlpensemble ensemble)
    {
        ensemble = new mlpensemble();
        mlpe.mlpecreatec1(nin, nhid, nout, ensemblesize, ensemble.innerobj);
        return;
    }

    /*************************************************************************
    Like MLPCreateC2, but for ensembles.

      -- ALGLIB --
         Copyright 18.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpecreatec2(int nin, int nhid1, int nhid2, int nout, int ensemblesize, out mlpensemble ensemble)
    {
        ensemble = new mlpensemble();
        mlpe.mlpecreatec2(nin, nhid1, nhid2, nout, ensemblesize, ensemble.innerobj);
        return;
    }

    /*************************************************************************
    Creates ensemble from network. Only network geometry is copied.

      -- ALGLIB --
         Copyright 17.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpecreatefromnetwork(multilayerperceptron network, int ensemblesize, out mlpensemble ensemble)
    {
        ensemble = new mlpensemble();
        mlpe.mlpecreatefromnetwork(network.innerobj, ensemblesize, ensemble.innerobj);
        return;
    }

    /*************************************************************************
    Randomization of MLP ensemble

      -- ALGLIB --
         Copyright 17.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlperandomize(mlpensemble ensemble)
    {

        mlpe.mlperandomize(ensemble.innerobj);
        return;
    }

    /*************************************************************************
    Return ensemble properties (number of inputs and outputs).

      -- ALGLIB --
         Copyright 17.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpeproperties(mlpensemble ensemble, out int nin, out int nout)
    {
        nin = 0;
        nout = 0;
        mlpe.mlpeproperties(ensemble.innerobj, ref nin, ref nout);
        return;
    }

    /*************************************************************************
    Return normalization type (whether ensemble is SOFTMAX-normalized or not).

      -- ALGLIB --
         Copyright 17.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static bool mlpeissoftmax(mlpensemble ensemble)
    {

        bool result = mlpe.mlpeissoftmax(ensemble.innerobj);
        return result;
    }

    /*************************************************************************
    Procesing

    INPUT PARAMETERS:
        Ensemble-   neural networks ensemble
        X       -   input vector,  array[0..NIn-1].
        Y       -   (possibly) preallocated buffer; if size of Y is less than
                    NOut, it will be reallocated. If it is large enough, it
                    is NOT reallocated, so we can save some time on reallocation.


    OUTPUT PARAMETERS:
        Y       -   result. Regression estimate when solving regression  task,
                    vector of posterior probabilities for classification task.

      -- ALGLIB --
         Copyright 17.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpeprocess(mlpensemble ensemble, double[] x, ref double[] y)
    {

        mlpe.mlpeprocess(ensemble.innerobj, x, ref y);
        return;
    }

    /*************************************************************************
    'interactive'  variant  of  MLPEProcess  for  languages  like Python which
    support constructs like "Y = MLPEProcess(LM,X)" and interactive mode of the
    interpreter

    This function allocates new array on each call,  so  it  is  significantly
    slower than its 'non-interactive' counterpart, but it is  more  convenient
    when you call it from command line.

      -- ALGLIB --
         Copyright 17.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpeprocessi(mlpensemble ensemble, double[] x, out double[] y)
    {
        y = new double[0];
        mlpe.mlpeprocessi(ensemble.innerobj, x, ref y);
        return;
    }

    /*************************************************************************
    Relative classification error on the test set

    INPUT PARAMETERS:
        Ensemble-   ensemble
        XY      -   test set
        NPoints -   test set size

    RESULT:
        percent of incorrectly classified cases.
        Works both for classifier betwork and for regression networks which
    are used as classifiers.

      -- ALGLIB --
         Copyright 17.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static double mlperelclserror(mlpensemble ensemble, double[,] xy, int npoints)
    {

        double result = mlpe.mlperelclserror(ensemble.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    Average cross-entropy (in bits per element) on the test set

    INPUT PARAMETERS:
        Ensemble-   ensemble
        XY      -   test set
        NPoints -   test set size

    RESULT:
        CrossEntropy/(NPoints*LN(2)).
        Zero if ensemble solves regression task.

      -- ALGLIB --
         Copyright 17.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static double mlpeavgce(mlpensemble ensemble, double[,] xy, int npoints)
    {

        double result = mlpe.mlpeavgce(ensemble.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    RMS error on the test set

    INPUT PARAMETERS:
        Ensemble-   ensemble
        XY      -   test set
        NPoints -   test set size

    RESULT:
        root mean square error.
        Its meaning for regression task is obvious. As for classification task
    RMS error means error when estimating posterior probabilities.

      -- ALGLIB --
         Copyright 17.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static double mlpermserror(mlpensemble ensemble, double[,] xy, int npoints)
    {

        double result = mlpe.mlpermserror(ensemble.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    Average error on the test set

    INPUT PARAMETERS:
        Ensemble-   ensemble
        XY      -   test set
        NPoints -   test set size

    RESULT:
        Its meaning for regression task is obvious. As for classification task
    it means average error when estimating posterior probabilities.

      -- ALGLIB --
         Copyright 17.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static double mlpeavgerror(mlpensemble ensemble, double[,] xy, int npoints)
    {

        double result = mlpe.mlpeavgerror(ensemble.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    Average relative error on the test set

    INPUT PARAMETERS:
        Ensemble-   ensemble
        XY      -   test set
        NPoints -   test set size

    RESULT:
        Its meaning for regression task is obvious. As for classification task
    it means average relative error when estimating posterior probabilities.

      -- ALGLIB --
         Copyright 17.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static double mlpeavgrelerror(mlpensemble ensemble, double[,] xy, int npoints)
    {

        double result = mlpe.mlpeavgrelerror(ensemble.innerobj, xy, npoints);
        return result;
    }

    /*************************************************************************
    Training neural networks ensemble using  bootstrap  aggregating (bagging).
    Modified Levenberg-Marquardt algorithm (AKA Damped Least Squares) is used as base training method.

    INPUT PARAMETERS:
        Ensemble    -   model with initialized geometry
        XY          -   training set
        NPoints     -   training set size
        Decay       -   weight decay coefficient, >=0.001
        Restarts    -   restarts, >0.

    OUTPUT PARAMETERS:
        Ensemble    -   trained model
        Info        -   return code:
                        * -2, if there is a point with class number
                              outside of [0..NClasses-1].
                        * -1, if incorrect parameters was passed
                              (NPoints<0, Restarts<1).
                        *  2, if task has been solved.
        Rep         -   training report.
        OOBErrors   -   out-of-bag generalization error estimate

      -- ALGLIB --
         Copyright 17.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpebagginglm(mlpensemble ensemble, double[,] xy, int npoints, double decay, int restarts, out int info, out mlpreport rep, out mlpcvreport ooberrors)
    {
        info = 0;
        rep = new mlpreport();
        ooberrors = new mlpcvreport();
        mlpe.mlpebagginglm(ensemble.innerobj, xy, npoints, decay, restarts, ref info, rep.innerobj, ooberrors.innerobj);
        return;
    }

    /*************************************************************************
    Training neural networks ensemble using  bootstrap  aggregating (bagging).
    L-BFGS algorithm is used as base training method.

    INPUT PARAMETERS:
        Ensemble    -   model with initialized geometry
        XY          -   training set
        NPoints     -   training set size
        Decay       -   weight decay coefficient, >=0.001
        Restarts    -   restarts, >0.
        WStep       -   stopping criterion, same as in MLPTrainLBFGS
        MaxIts      -   stopping criterion, same as in MLPTrainLBFGS

    OUTPUT PARAMETERS:
        Ensemble    -   trained model
        Info        -   return code:
                        * -8, if both WStep=0 and MaxIts=0
                        * -2, if there is a point with class number
                              outside of [0..NClasses-1].
                        * -1, if incorrect parameters was passed
                              (NPoints<0, Restarts<1).
                        *  2, if task has been solved.
        Rep         -   training report.
        OOBErrors   -   out-of-bag generalization error estimate

      -- ALGLIB --
         Copyright 17.02.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpebagginglbfgs(mlpensemble ensemble, double[,] xy, int npoints, double decay, int restarts, double wstep, int maxits, out int info, out mlpreport rep, out mlpcvreport ooberrors)
    {
        info = 0;
        rep = new mlpreport();
        ooberrors = new mlpcvreport();
        mlpe.mlpebagginglbfgs(ensemble.innerobj, xy, npoints, decay, restarts, wstep, maxits, ref info, rep.innerobj, ooberrors.innerobj);
        return;
    }

    /*************************************************************************
    Training neural networks ensemble using early stopping.

    INPUT PARAMETERS:
        Ensemble    -   model with initialized geometry
        XY          -   training set
        NPoints     -   training set size
        Decay       -   weight decay coefficient, >=0.001
        Restarts    -   restarts, >0.

    OUTPUT PARAMETERS:
        Ensemble    -   trained model
        Info        -   return code:
                        * -2, if there is a point with class number
                              outside of [0..NClasses-1].
                        * -1, if incorrect parameters was passed
                              (NPoints<0, Restarts<1).
                        *  6, if task has been solved.
        Rep         -   training report.
        OOBErrors   -   out-of-bag generalization error estimate

      -- ALGLIB --
         Copyright 10.03.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mlpetraines(mlpensemble ensemble, double[,] xy, int npoints, double decay, int restarts, out int info, out mlpreport rep)
    {
        info = 0;
        rep = new mlpreport();
        mlpe.mlpetraines(ensemble.innerobj, xy, npoints, decay, restarts, ref info, rep.innerobj);
        return;
    }

}
public partial class alglib
{


    /*************************************************************************
    Principal components analysis

    Subroutine  builds  orthogonal  basis  where  first  axis  corresponds  to
    direction with maximum variance, second axis maximizes variance in subspace
    orthogonal to first axis and so on.

    It should be noted that, unlike LDA, PCA does not use class labels.

    INPUT PARAMETERS:
        X           -   dataset, array[0..NPoints-1,0..NVars-1].
                        matrix contains ONLY INDEPENDENT VARIABLES.
        NPoints     -   dataset size, NPoints>=0
        NVars       -   number of independent variables, NVars>=1

    �������� ���������:
        Info        -   return code:
                        * -4, if SVD subroutine haven't converged
                        * -1, if wrong parameters has been passed (NPoints<0,
                              NVars<1)
                        *  1, if task is solved
        S2          -   array[0..NVars-1]. variance values corresponding
                        to basis vectors.
        V           -   array[0..NVars-1,0..NVars-1]
                        matrix, whose columns store basis vectors.

      -- ALGLIB --
         Copyright 25.08.2008 by Bochkanov Sergey
    *************************************************************************/
    public static void pcabuildbasis(double[,] x, int npoints, int nvars, out int info, out double[] s2, out double[,] v)
    {
        info = 0;
        s2 = new double[0];
        v = new double[0,0];
        pca.pcabuildbasis(x, npoints, nvars, ref info, ref s2, ref v);
        return;
    }

}
public partial class alglib
{
    public class bdss
    {
        public class cvreport
        {
            public double relclserror;
            public double avgce;
            public double rmserror;
            public double avgerror;
            public double avgrelerror;
        };




        /*************************************************************************
        This set of routines (DSErrAllocate, DSErrAccumulate, DSErrFinish)
        calculates different error functions (classification error, cross-entropy,
        rms, avg, avg.rel errors).

        1. DSErrAllocate prepares buffer.
        2. DSErrAccumulate accumulates individual errors:
            * Y contains predicted output (posterior probabilities for classification)
            * DesiredY contains desired output (class number for classification)
        3. DSErrFinish outputs results:
           * Buf[0] contains relative classification error (zero for regression tasks)
           * Buf[1] contains avg. cross-entropy (zero for regression tasks)
           * Buf[2] contains rms error (regression, classification)
           * Buf[3] contains average error (regression, classification)
           * Buf[4] contains average relative error (regression, classification)
           
        NOTES(1):
            "NClasses>0" means that we have classification task.
            "NClasses<0" means regression task with -NClasses real outputs.

        NOTES(2):
            rms. avg, avg.rel errors for classification tasks are interpreted as
            errors in posterior probabilities with respect to probabilities given
            by training/test set.

          -- ALGLIB --
             Copyright 11.01.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void dserrallocate(int nclasses,
            ref double[] buf)
        {
            buf = new double[0];

            buf = new double[7+1];
            buf[0] = 0;
            buf[1] = 0;
            buf[2] = 0;
            buf[3] = 0;
            buf[4] = 0;
            buf[5] = nclasses;
            buf[6] = 0;
            buf[7] = 0;
        }


        /*************************************************************************
        See DSErrAllocate for comments on this routine.

          -- ALGLIB --
             Copyright 11.01.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void dserraccumulate(ref double[] buf,
            double[] y,
            double[] desiredy)
        {
            int nclasses = 0;
            int nout = 0;
            int offs = 0;
            int mmax = 0;
            int rmax = 0;
            int j = 0;
            double v = 0;
            double ev = 0;

            offs = 5;
            nclasses = (int)Math.Round(buf[offs]);
            if( nclasses>0 )
            {
                
                //
                // Classification
                //
                rmax = (int)Math.Round(desiredy[0]);
                mmax = 0;
                for(j=1; j<=nclasses-1; j++)
                {
                    if( (double)(y[j])>(double)(y[mmax]) )
                    {
                        mmax = j;
                    }
                }
                if( mmax!=rmax )
                {
                    buf[0] = buf[0]+1;
                }
                if( (double)(y[rmax])>(double)(0) )
                {
                    buf[1] = buf[1]-Math.Log(y[rmax]);
                }
                else
                {
                    buf[1] = buf[1]+Math.Log(math.maxrealnumber);
                }
                for(j=0; j<=nclasses-1; j++)
                {
                    v = y[j];
                    if( j==rmax )
                    {
                        ev = 1;
                    }
                    else
                    {
                        ev = 0;
                    }
                    buf[2] = buf[2]+math.sqr(v-ev);
                    buf[3] = buf[3]+Math.Abs(v-ev);
                    if( (double)(ev)!=(double)(0) )
                    {
                        buf[4] = buf[4]+Math.Abs((v-ev)/ev);
                        buf[offs+2] = buf[offs+2]+1;
                    }
                }
                buf[offs+1] = buf[offs+1]+1;
            }
            else
            {
                
                //
                // Regression
                //
                nout = -nclasses;
                rmax = 0;
                for(j=1; j<=nout-1; j++)
                {
                    if( (double)(desiredy[j])>(double)(desiredy[rmax]) )
                    {
                        rmax = j;
                    }
                }
                mmax = 0;
                for(j=1; j<=nout-1; j++)
                {
                    if( (double)(y[j])>(double)(y[mmax]) )
                    {
                        mmax = j;
                    }
                }
                if( mmax!=rmax )
                {
                    buf[0] = buf[0]+1;
                }
                for(j=0; j<=nout-1; j++)
                {
                    v = y[j];
                    ev = desiredy[j];
                    buf[2] = buf[2]+math.sqr(v-ev);
                    buf[3] = buf[3]+Math.Abs(v-ev);
                    if( (double)(ev)!=(double)(0) )
                    {
                        buf[4] = buf[4]+Math.Abs((v-ev)/ev);
                        buf[offs+2] = buf[offs+2]+1;
                    }
                }
                buf[offs+1] = buf[offs+1]+1;
            }
        }


        /*************************************************************************
        See DSErrAllocate for comments on this routine.

          -- ALGLIB --
             Copyright 11.01.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void dserrfinish(ref double[] buf)
        {
            int nout = 0;
            int offs = 0;

            offs = 5;
            nout = Math.Abs((int)Math.Round(buf[offs]));
            if( (double)(buf[offs+1])!=(double)(0) )
            {
                buf[0] = buf[0]/buf[offs+1];
                buf[1] = buf[1]/buf[offs+1];
                buf[2] = Math.Sqrt(buf[2]/(nout*buf[offs+1]));
                buf[3] = buf[3]/(nout*buf[offs+1]);
            }
            if( (double)(buf[offs+2])!=(double)(0) )
            {
                buf[4] = buf[4]/buf[offs+2];
            }
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 19.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dsnormalize(ref double[,] xy,
            int npoints,
            int nvars,
            ref int info,
            ref double[] means,
            ref double[] sigmas)
        {
            int i = 0;
            int j = 0;
            double[] tmp = new double[0];
            double mean = 0;
            double variance = 0;
            double skewness = 0;
            double kurtosis = 0;
            int i_ = 0;

            info = 0;
            means = new double[0];
            sigmas = new double[0];

            
            //
            // Test parameters
            //
            if( npoints<=0 | nvars<1 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // Standartization
            //
            means = new double[nvars-1+1];
            sigmas = new double[nvars-1+1];
            tmp = new double[npoints-1+1];
            for(j=0; j<=nvars-1; j++)
            {
                for(i_=0; i_<=npoints-1;i_++)
                {
                    tmp[i_] = xy[i_,j];
                }
                basestat.samplemoments(tmp, npoints, ref mean, ref variance, ref skewness, ref kurtosis);
                means[j] = mean;
                sigmas[j] = Math.Sqrt(variance);
                if( (double)(sigmas[j])==(double)(0) )
                {
                    sigmas[j] = 1;
                }
                for(i=0; i<=npoints-1; i++)
                {
                    xy[i,j] = (xy[i,j]-means[j])/sigmas[j];
                }
            }
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 19.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dsnormalizec(double[,] xy,
            int npoints,
            int nvars,
            ref int info,
            ref double[] means,
            ref double[] sigmas)
        {
            int j = 0;
            double[] tmp = new double[0];
            double mean = 0;
            double variance = 0;
            double skewness = 0;
            double kurtosis = 0;
            int i_ = 0;

            info = 0;
            means = new double[0];
            sigmas = new double[0];

            
            //
            // Test parameters
            //
            if( npoints<=0 | nvars<1 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // Standartization
            //
            means = new double[nvars-1+1];
            sigmas = new double[nvars-1+1];
            tmp = new double[npoints-1+1];
            for(j=0; j<=nvars-1; j++)
            {
                for(i_=0; i_<=npoints-1;i_++)
                {
                    tmp[i_] = xy[i_,j];
                }
                basestat.samplemoments(tmp, npoints, ref mean, ref variance, ref skewness, ref kurtosis);
                means[j] = mean;
                sigmas[j] = Math.Sqrt(variance);
                if( (double)(sigmas[j])==(double)(0) )
                {
                    sigmas[j] = 1;
                }
            }
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 19.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double dsgetmeanmindistance(double[,] xy,
            int npoints,
            int nvars)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double[] tmp = new double[0];
            double[] tmp2 = new double[0];
            double v = 0;
            int i_ = 0;

            
            //
            // Test parameters
            //
            if( npoints<=0 | nvars<1 )
            {
                result = 0;
                return result;
            }
            
            //
            // Process
            //
            tmp = new double[npoints-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                tmp[i] = math.maxrealnumber;
            }
            tmp2 = new double[nvars-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                for(j=i+1; j<=npoints-1; j++)
                {
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        tmp2[i_] = xy[i,i_];
                    }
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        tmp2[i_] = tmp2[i_] - xy[j,i_];
                    }
                    v = 0.0;
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        v += tmp2[i_]*tmp2[i_];
                    }
                    v = Math.Sqrt(v);
                    tmp[i] = Math.Min(tmp[i], v);
                    tmp[j] = Math.Min(tmp[j], v);
                }
            }
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                result = result+tmp[i]/npoints;
            }
            return result;
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 19.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dstie(ref double[] a,
            int n,
            ref int[] ties,
            ref int tiecount,
            ref int[] p1,
            ref int[] p2)
        {
            int i = 0;
            int k = 0;
            int[] tmp = new int[0];

            ties = new int[0];
            tiecount = 0;
            p1 = new int[0];
            p2 = new int[0];

            
            //
            // Special case
            //
            if( n<=0 )
            {
                tiecount = 0;
                return;
            }
            
            //
            // Sort A
            //
            tsort.tagsort(ref a, n, ref p1, ref p2);
            
            //
            // Process ties
            //
            tiecount = 1;
            for(i=1; i<=n-1; i++)
            {
                if( (double)(a[i])!=(double)(a[i-1]) )
                {
                    tiecount = tiecount+1;
                }
            }
            ties = new int[tiecount+1];
            ties[0] = 0;
            k = 1;
            for(i=1; i<=n-1; i++)
            {
                if( (double)(a[i])!=(double)(a[i-1]) )
                {
                    ties[k] = i;
                    k = k+1;
                }
            }
            ties[tiecount] = n;
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 11.12.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dstiefasti(ref double[] a,
            ref int[] b,
            int n,
            ref int[] ties,
            ref int tiecount,
            ref double[] bufr,
            ref int[] bufi)
        {
            int i = 0;
            int k = 0;
            int[] tmp = new int[0];

            tiecount = 0;

            
            //
            // Special case
            //
            if( n<=0 )
            {
                tiecount = 0;
                return;
            }
            
            //
            // Sort A
            //
            tsort.tagsortfasti(ref a, ref b, ref bufr, ref bufi, n);
            
            //
            // Process ties
            //
            ties[0] = 0;
            k = 1;
            for(i=1; i<=n-1; i++)
            {
                if( (double)(a[i])!=(double)(a[i-1]) )
                {
                    ties[k] = i;
                    k = k+1;
                }
            }
            ties[k] = n;
            tiecount = k;
        }


        /*************************************************************************
        Optimal binary classification

        Algorithms finds optimal (=with minimal cross-entropy) binary partition.
        Internal subroutine.

        INPUT PARAMETERS:
            A       -   array[0..N-1], variable
            C       -   array[0..N-1], class numbers (0 or 1).
            N       -   array size

        OUTPUT PARAMETERS:
            Info    -   completetion code:
                        * -3, all values of A[] are same (partition is impossible)
                        * -2, one of C[] is incorrect (<0, >1)
                        * -1, incorrect pararemets were passed (N<=0).
                        *  1, OK
            Threshold-  partiton boundary. Left part contains values which are
                        strictly less than Threshold. Right part contains values
                        which are greater than or equal to Threshold.
            PAL, PBL-   probabilities P(0|v<Threshold) and P(1|v<Threshold)
            PAR, PBR-   probabilities P(0|v>=Threshold) and P(1|v>=Threshold)
            CVE     -   cross-validation estimate of cross-entropy

          -- ALGLIB --
             Copyright 22.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dsoptimalsplit2(double[] a,
            int[] c,
            int n,
            ref int info,
            ref double threshold,
            ref double pal,
            ref double pbl,
            ref double par,
            ref double pbr,
            ref double cve)
        {
            int i = 0;
            int t = 0;
            double s = 0;
            int[] ties = new int[0];
            int tiecount = 0;
            int[] p1 = new int[0];
            int[] p2 = new int[0];
            int k = 0;
            int koptimal = 0;
            double pak = 0;
            double pbk = 0;
            double cvoptimal = 0;
            double cv = 0;

            a = (double[])a.Clone();
            c = (int[])c.Clone();
            info = 0;
            threshold = 0;
            pal = 0;
            pbl = 0;
            par = 0;
            pbr = 0;
            cve = 0;

            
            //
            // Test for errors in inputs
            //
            if( n<=0 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( c[i]!=0 & c[i]!=1 )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // Tie
            //
            dstie(ref a, n, ref ties, ref tiecount, ref p1, ref p2);
            for(i=0; i<=n-1; i++)
            {
                if( p2[i]!=i )
                {
                    t = c[i];
                    c[i] = c[p2[i]];
                    c[p2[i]] = t;
                }
            }
            
            //
            // Special case: number of ties is 1.
            //
            // NOTE: we assume that P[i,j] equals to 0 or 1,
            //       intermediate values are not allowed.
            //
            if( tiecount==1 )
            {
                info = -3;
                return;
            }
            
            //
            // General case, number of ties > 1
            //
            // NOTE: we assume that P[i,j] equals to 0 or 1,
            //       intermediate values are not allowed.
            //
            pal = 0;
            pbl = 0;
            par = 0;
            pbr = 0;
            for(i=0; i<=n-1; i++)
            {
                if( c[i]==0 )
                {
                    par = par+1;
                }
                if( c[i]==1 )
                {
                    pbr = pbr+1;
                }
            }
            koptimal = -1;
            cvoptimal = math.maxrealnumber;
            for(k=0; k<=tiecount-2; k++)
            {
                
                //
                // first, obtain information about K-th tie which is
                // moved from R-part to L-part
                //
                pak = 0;
                pbk = 0;
                for(i=ties[k]; i<=ties[k+1]-1; i++)
                {
                    if( c[i]==0 )
                    {
                        pak = pak+1;
                    }
                    if( c[i]==1 )
                    {
                        pbk = pbk+1;
                    }
                }
                
                //
                // Calculate cross-validation CE
                //
                cv = 0;
                cv = cv-xlny(pal+pak, (pal+pak)/(pal+pak+pbl+pbk+1));
                cv = cv-xlny(pbl+pbk, (pbl+pbk)/(pal+pak+1+pbl+pbk));
                cv = cv-xlny(par-pak, (par-pak)/(par-pak+pbr-pbk+1));
                cv = cv-xlny(pbr-pbk, (pbr-pbk)/(par-pak+1+pbr-pbk));
                
                //
                // Compare with best
                //
                if( (double)(cv)<(double)(cvoptimal) )
                {
                    cvoptimal = cv;
                    koptimal = k;
                }
                
                //
                // update
                //
                pal = pal+pak;
                pbl = pbl+pbk;
                par = par-pak;
                pbr = pbr-pbk;
            }
            cve = cvoptimal;
            threshold = 0.5*(a[ties[koptimal]]+a[ties[koptimal+1]]);
            pal = 0;
            pbl = 0;
            par = 0;
            pbr = 0;
            for(i=0; i<=n-1; i++)
            {
                if( (double)(a[i])<(double)(threshold) )
                {
                    if( c[i]==0 )
                    {
                        pal = pal+1;
                    }
                    else
                    {
                        pbl = pbl+1;
                    }
                }
                else
                {
                    if( c[i]==0 )
                    {
                        par = par+1;
                    }
                    else
                    {
                        pbr = pbr+1;
                    }
                }
            }
            s = pal+pbl;
            pal = pal/s;
            pbl = pbl/s;
            s = par+pbr;
            par = par/s;
            pbr = pbr/s;
        }


        /*************************************************************************
        Optimal partition, internal subroutine. Fast version.

        Accepts:
            A       array[0..N-1]       array of attributes     array[0..N-1]
            C       array[0..N-1]       array of class labels
            TiesBuf array[0..N]         temporaries (ties)
            CntBuf  array[0..2*NC-1]    temporaries (counts)
            Alpha                       centering factor (0<=alpha<=1, recommended value - 0.05)
            BufR    array[0..N-1]       temporaries
            BufI    array[0..N-1]       temporaries

        Output:
            Info    error code (">0"=OK, "<0"=bad)
            RMS     training set RMS error
            CVRMS   leave-one-out RMS error
            
        Note:
            content of all arrays is changed by subroutine;
            it doesn't allocate temporaries.

          -- ALGLIB --
             Copyright 11.12.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dsoptimalsplit2fast(ref double[] a,
            ref int[] c,
            ref int[] tiesbuf,
            ref int[] cntbuf,
            ref double[] bufr,
            ref int[] bufi,
            int n,
            int nc,
            double alpha,
            ref int info,
            ref double threshold,
            ref double rms,
            ref double cvrms)
        {
            int i = 0;
            int k = 0;
            int cl = 0;
            int tiecount = 0;
            double cbest = 0;
            double cc = 0;
            int koptimal = 0;
            int sl = 0;
            int sr = 0;
            double v = 0;
            double w = 0;
            double x = 0;

            info = 0;
            threshold = 0;
            rms = 0;
            cvrms = 0;

            
            //
            // Test for errors in inputs
            //
            if( n<=0 | nc<2 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( c[i]<0 | c[i]>=nc )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // Tie
            //
            dstiefasti(ref a, ref c, n, ref tiesbuf, ref tiecount, ref bufr, ref bufi);
            
            //
            // Special case: number of ties is 1.
            //
            if( tiecount==1 )
            {
                info = -3;
                return;
            }
            
            //
            // General case, number of ties > 1
            //
            for(i=0; i<=2*nc-1; i++)
            {
                cntbuf[i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                cntbuf[nc+c[i]] = cntbuf[nc+c[i]]+1;
            }
            koptimal = -1;
            threshold = a[n-1];
            cbest = math.maxrealnumber;
            sl = 0;
            sr = n;
            for(k=0; k<=tiecount-2; k++)
            {
                
                //
                // first, move Kth tie from right to left
                //
                for(i=tiesbuf[k]; i<=tiesbuf[k+1]-1; i++)
                {
                    cl = c[i];
                    cntbuf[cl] = cntbuf[cl]+1;
                    cntbuf[nc+cl] = cntbuf[nc+cl]-1;
                }
                sl = sl+(tiesbuf[k+1]-tiesbuf[k]);
                sr = sr-(tiesbuf[k+1]-tiesbuf[k]);
                
                //
                // Calculate RMS error
                //
                v = 0;
                for(i=0; i<=nc-1; i++)
                {
                    w = cntbuf[i];
                    v = v+w*math.sqr(w/sl-1);
                    v = v+(sl-w)*math.sqr(w/sl);
                    w = cntbuf[nc+i];
                    v = v+w*math.sqr(w/sr-1);
                    v = v+(sr-w)*math.sqr(w/sr);
                }
                v = Math.Sqrt(v/(nc*n));
                
                //
                // Compare with best
                //
                x = (double)(2*sl)/(double)(sl+sr)-1;
                cc = v*(1-alpha+alpha*math.sqr(x));
                if( (double)(cc)<(double)(cbest) )
                {
                    
                    //
                    // store split
                    //
                    rms = v;
                    koptimal = k;
                    cbest = cc;
                    
                    //
                    // calculate CVRMS error
                    //
                    cvrms = 0;
                    for(i=0; i<=nc-1; i++)
                    {
                        if( sl>1 )
                        {
                            w = cntbuf[i];
                            cvrms = cvrms+w*math.sqr((w-1)/(sl-1)-1);
                            cvrms = cvrms+(sl-w)*math.sqr(w/(sl-1));
                        }
                        else
                        {
                            w = cntbuf[i];
                            cvrms = cvrms+w*math.sqr((double)1/(double)nc-1);
                            cvrms = cvrms+(sl-w)*math.sqr((double)1/(double)nc);
                        }
                        if( sr>1 )
                        {
                            w = cntbuf[nc+i];
                            cvrms = cvrms+w*math.sqr((w-1)/(sr-1)-1);
                            cvrms = cvrms+(sr-w)*math.sqr(w/(sr-1));
                        }
                        else
                        {
                            w = cntbuf[nc+i];
                            cvrms = cvrms+w*math.sqr((double)1/(double)nc-1);
                            cvrms = cvrms+(sr-w)*math.sqr((double)1/(double)nc);
                        }
                    }
                    cvrms = Math.Sqrt(cvrms/(nc*n));
                }
            }
            
            //
            // Calculate threshold.
            // Code is a bit complicated because there can be such
            // numbers that 0.5(A+B) equals to A or B (if A-B=epsilon)
            //
            threshold = 0.5*(a[tiesbuf[koptimal]]+a[tiesbuf[koptimal+1]]);
            if( (double)(threshold)<=(double)(a[tiesbuf[koptimal]]) )
            {
                threshold = a[tiesbuf[koptimal+1]];
            }
        }


        /*************************************************************************
        Automatic non-optimal discretization, internal subroutine.

          -- ALGLIB --
             Copyright 22.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dssplitk(double[] a,
            int[] c,
            int n,
            int nc,
            int kmax,
            ref int info,
            ref double[] thresholds,
            ref int ni,
            ref double cve)
        {
            int i = 0;
            int j = 0;
            int j1 = 0;
            int k = 0;
            int[] ties = new int[0];
            int tiecount = 0;
            int[] p1 = new int[0];
            int[] p2 = new int[0];
            int[] cnt = new int[0];
            double v2 = 0;
            int bestk = 0;
            double bestcve = 0;
            int[] bestsizes = new int[0];
            double curcve = 0;
            int[] cursizes = new int[0];

            a = (double[])a.Clone();
            c = (int[])c.Clone();
            info = 0;
            thresholds = new double[0];
            ni = 0;
            cve = 0;

            
            //
            // Test for errors in inputs
            //
            if( (n<=0 | nc<2) | kmax<2 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( c[i]<0 | c[i]>=nc )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // Tie
            //
            dstie(ref a, n, ref ties, ref tiecount, ref p1, ref p2);
            for(i=0; i<=n-1; i++)
            {
                if( p2[i]!=i )
                {
                    k = c[i];
                    c[i] = c[p2[i]];
                    c[p2[i]] = k;
                }
            }
            
            //
            // Special cases
            //
            if( tiecount==1 )
            {
                info = -3;
                return;
            }
            
            //
            // General case:
            // 0. allocate arrays
            //
            kmax = Math.Min(kmax, tiecount);
            bestsizes = new int[kmax-1+1];
            cursizes = new int[kmax-1+1];
            cnt = new int[nc-1+1];
            
            //
            // General case:
            // 1. prepare "weak" solution (two subintervals, divided at median)
            //
            v2 = math.maxrealnumber;
            j = -1;
            for(i=1; i<=tiecount-1; i++)
            {
                if( (double)(Math.Abs(ties[i]-0.5*(n-1)))<(double)(v2) )
                {
                    v2 = Math.Abs(ties[i]-0.5*n);
                    j = i;
                }
            }
            ap.assert(j>0, "DSSplitK: internal error #1!");
            bestk = 2;
            bestsizes[0] = ties[j];
            bestsizes[1] = n-j;
            bestcve = 0;
            for(i=0; i<=nc-1; i++)
            {
                cnt[i] = 0;
            }
            for(i=0; i<=j-1; i++)
            {
                tieaddc(c, ties, i, nc, ref cnt);
            }
            bestcve = bestcve+getcv(cnt, nc);
            for(i=0; i<=nc-1; i++)
            {
                cnt[i] = 0;
            }
            for(i=j; i<=tiecount-1; i++)
            {
                tieaddc(c, ties, i, nc, ref cnt);
            }
            bestcve = bestcve+getcv(cnt, nc);
            
            //
            // General case:
            // 2. Use greedy algorithm to find sub-optimal split in O(KMax*N) time
            //
            for(k=2; k<=kmax; k++)
            {
                
                //
                // Prepare greedy K-interval split
                //
                for(i=0; i<=k-1; i++)
                {
                    cursizes[i] = 0;
                }
                i = 0;
                j = 0;
                while( j<=tiecount-1 & i<=k-1 )
                {
                    
                    //
                    // Rule: I-th bin is empty, fill it
                    //
                    if( cursizes[i]==0 )
                    {
                        cursizes[i] = ties[j+1]-ties[j];
                        j = j+1;
                        continue;
                    }
                    
                    //
                    // Rule: (K-1-I) bins left, (K-1-I) ties left (1 tie per bin); next bin
                    //
                    if( tiecount-j==k-1-i )
                    {
                        i = i+1;
                        continue;
                    }
                    
                    //
                    // Rule: last bin, always place in current
                    //
                    if( i==k-1 )
                    {
                        cursizes[i] = cursizes[i]+ties[j+1]-ties[j];
                        j = j+1;
                        continue;
                    }
                    
                    //
                    // Place J-th tie in I-th bin, or leave for I+1-th bin.
                    //
                    if( (double)(Math.Abs(cursizes[i]+ties[j+1]-ties[j]-(double)n/(double)k))<(double)(Math.Abs(cursizes[i]-(double)n/(double)k)) )
                    {
                        cursizes[i] = cursizes[i]+ties[j+1]-ties[j];
                        j = j+1;
                    }
                    else
                    {
                        i = i+1;
                    }
                }
                ap.assert(cursizes[k-1]!=0 & j==tiecount, "DSSplitK: internal error #1");
                
                //
                // Calculate CVE
                //
                curcve = 0;
                j = 0;
                for(i=0; i<=k-1; i++)
                {
                    for(j1=0; j1<=nc-1; j1++)
                    {
                        cnt[j1] = 0;
                    }
                    for(j1=j; j1<=j+cursizes[i]-1; j1++)
                    {
                        cnt[c[j1]] = cnt[c[j1]]+1;
                    }
                    curcve = curcve+getcv(cnt, nc);
                    j = j+cursizes[i];
                }
                
                //
                // Choose best variant
                //
                if( (double)(curcve)<(double)(bestcve) )
                {
                    for(i=0; i<=k-1; i++)
                    {
                        bestsizes[i] = cursizes[i];
                    }
                    bestcve = curcve;
                    bestk = k;
                }
            }
            
            //
            // Transform from sizes to thresholds
            //
            cve = bestcve;
            ni = bestk;
            thresholds = new double[ni-2+1];
            j = bestsizes[0];
            for(i=1; i<=bestk-1; i++)
            {
                thresholds[i-1] = 0.5*(a[j-1]+a[j]);
                j = j+bestsizes[i];
            }
        }


        /*************************************************************************
        Automatic optimal discretization, internal subroutine.

          -- ALGLIB --
             Copyright 22.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dsoptimalsplitk(double[] a,
            int[] c,
            int n,
            int nc,
            int kmax,
            ref int info,
            ref double[] thresholds,
            ref int ni,
            ref double cve)
        {
            int i = 0;
            int j = 0;
            int s = 0;
            int jl = 0;
            int jr = 0;
            double v2 = 0;
            int[] ties = new int[0];
            int tiecount = 0;
            int[] p1 = new int[0];
            int[] p2 = new int[0];
            double cvtemp = 0;
            int[] cnt = new int[0];
            int[] cnt2 = new int[0];
            double[,] cv = new double[0,0];
            int[,] splits = new int[0,0];
            int k = 0;
            int koptimal = 0;
            double cvoptimal = 0;

            a = (double[])a.Clone();
            c = (int[])c.Clone();
            info = 0;
            thresholds = new double[0];
            ni = 0;
            cve = 0;

            
            //
            // Test for errors in inputs
            //
            if( (n<=0 | nc<2) | kmax<2 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( c[i]<0 | c[i]>=nc )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // Tie
            //
            dstie(ref a, n, ref ties, ref tiecount, ref p1, ref p2);
            for(i=0; i<=n-1; i++)
            {
                if( p2[i]!=i )
                {
                    k = c[i];
                    c[i] = c[p2[i]];
                    c[p2[i]] = k;
                }
            }
            
            //
            // Special cases
            //
            if( tiecount==1 )
            {
                info = -3;
                return;
            }
            
            //
            // General case
            // Use dynamic programming to find best split in O(KMax*NC*TieCount^2) time
            //
            kmax = Math.Min(kmax, tiecount);
            cv = new double[kmax-1+1, tiecount-1+1];
            splits = new int[kmax-1+1, tiecount-1+1];
            cnt = new int[nc-1+1];
            cnt2 = new int[nc-1+1];
            for(j=0; j<=nc-1; j++)
            {
                cnt[j] = 0;
            }
            for(j=0; j<=tiecount-1; j++)
            {
                tieaddc(c, ties, j, nc, ref cnt);
                splits[0,j] = 0;
                cv[0,j] = getcv(cnt, nc);
            }
            for(k=1; k<=kmax-1; k++)
            {
                for(j=0; j<=nc-1; j++)
                {
                    cnt[j] = 0;
                }
                
                //
                // Subtask size J in [K..TieCount-1]:
                // optimal K-splitting on ties from 0-th to J-th.
                //
                for(j=k; j<=tiecount-1; j++)
                {
                    
                    //
                    // Update Cnt - let it contain classes of ties from K-th to J-th
                    //
                    tieaddc(c, ties, j, nc, ref cnt);
                    
                    //
                    // Search for optimal split point S in [K..J]
                    //
                    for(i=0; i<=nc-1; i++)
                    {
                        cnt2[i] = cnt[i];
                    }
                    cv[k,j] = cv[k-1,j-1]+getcv(cnt2, nc);
                    splits[k,j] = j;
                    for(s=k+1; s<=j; s++)
                    {
                        
                        //
                        // Update Cnt2 - let it contain classes of ties from S-th to J-th
                        //
                        tiesubc(c, ties, s-1, nc, ref cnt2);
                        
                        //
                        // Calculate CVE
                        //
                        cvtemp = cv[k-1,s-1]+getcv(cnt2, nc);
                        if( (double)(cvtemp)<(double)(cv[k,j]) )
                        {
                            cv[k,j] = cvtemp;
                            splits[k,j] = s;
                        }
                    }
                }
            }
            
            //
            // Choose best partition, output result
            //
            koptimal = -1;
            cvoptimal = math.maxrealnumber;
            for(k=0; k<=kmax-1; k++)
            {
                if( (double)(cv[k,tiecount-1])<(double)(cvoptimal) )
                {
                    cvoptimal = cv[k,tiecount-1];
                    koptimal = k;
                }
            }
            ap.assert(koptimal>=0, "DSOptimalSplitK: internal error #1!");
            if( koptimal==0 )
            {
                
                //
                // Special case: best partition is one big interval.
                // Even 2-partition is not better.
                // This is possible when dealing with "weak" predictor variables.
                //
                // Make binary split as close to the median as possible.
                //
                v2 = math.maxrealnumber;
                j = -1;
                for(i=1; i<=tiecount-1; i++)
                {
                    if( (double)(Math.Abs(ties[i]-0.5*(n-1)))<(double)(v2) )
                    {
                        v2 = Math.Abs(ties[i]-0.5*(n-1));
                        j = i;
                    }
                }
                ap.assert(j>0, "DSOptimalSplitK: internal error #2!");
                thresholds = new double[0+1];
                thresholds[0] = 0.5*(a[ties[j-1]]+a[ties[j]]);
                ni = 2;
                cve = 0;
                for(i=0; i<=nc-1; i++)
                {
                    cnt[i] = 0;
                }
                for(i=0; i<=j-1; i++)
                {
                    tieaddc(c, ties, i, nc, ref cnt);
                }
                cve = cve+getcv(cnt, nc);
                for(i=0; i<=nc-1; i++)
                {
                    cnt[i] = 0;
                }
                for(i=j; i<=tiecount-1; i++)
                {
                    tieaddc(c, ties, i, nc, ref cnt);
                }
                cve = cve+getcv(cnt, nc);
            }
            else
            {
                
                //
                // General case: 2 or more intervals
                //
                thresholds = new double[koptimal-1+1];
                ni = koptimal+1;
                cve = cv[koptimal,tiecount-1];
                jl = splits[koptimal,tiecount-1];
                jr = tiecount-1;
                for(k=koptimal; k>=1; k--)
                {
                    thresholds[k-1] = 0.5*(a[ties[jl-1]]+a[ties[jl]]);
                    jr = jl-1;
                    jl = splits[k-1,jl-1];
                }
            }
        }


        /*************************************************************************
        Internal function
        *************************************************************************/
        private static double xlny(double x,
            double y)
        {
            double result = 0;

            if( (double)(x)==(double)(0) )
            {
                result = 0;
            }
            else
            {
                result = x*Math.Log(y);
            }
            return result;
        }


        /*************************************************************************
        Internal function,
        returns number of samples of class I in Cnt[I]
        *************************************************************************/
        private static double getcv(int[] cnt,
            int nc)
        {
            double result = 0;
            int i = 0;
            double s = 0;

            s = 0;
            for(i=0; i<=nc-1; i++)
            {
                s = s+cnt[i];
            }
            result = 0;
            for(i=0; i<=nc-1; i++)
            {
                result = result-xlny(cnt[i], cnt[i]/(s+nc-1));
            }
            return result;
        }


        /*************************************************************************
        Internal function, adds number of samples of class I in tie NTie to Cnt[I]
        *************************************************************************/
        private static void tieaddc(int[] c,
            int[] ties,
            int ntie,
            int nc,
            ref int[] cnt)
        {
            int i = 0;

            for(i=ties[ntie]; i<=ties[ntie+1]-1; i++)
            {
                cnt[c[i]] = cnt[c[i]]+1;
            }
        }


        /*************************************************************************
        Internal function, subtracts number of samples of class I in tie NTie to Cnt[I]
        *************************************************************************/
        private static void tiesubc(int[] c,
            int[] ties,
            int ntie,
            int nc,
            ref int[] cnt)
        {
            int i = 0;

            for(i=ties[ntie]; i<=ties[ntie+1]-1; i++)
            {
                cnt[c[i]] = cnt[c[i]]-1;
            }
        }


    }
    public class dforest
    {
        public class decisionforest
        {
            public int nvars;
            public int nclasses;
            public int ntrees;
            public int bufsize;
            public double[] trees;
            public decisionforest()
            {
                trees = new double[0];
            }
        };


        public class dfreport
        {
            public double relclserror;
            public double avgce;
            public double rmserror;
            public double avgerror;
            public double avgrelerror;
            public double oobrelclserror;
            public double oobavgce;
            public double oobrmserror;
            public double oobavgerror;
            public double oobavgrelerror;
        };


        public class dfinternalbuffers
        {
            public double[] treebuf;
            public int[] idxbuf;
            public double[] tmpbufr;
            public double[] tmpbufr2;
            public int[] tmpbufi;
            public int[] classibuf;
            public double[] sortrbuf;
            public double[] sortrbuf2;
            public int[] sortibuf;
            public int[] varpool;
            public bool[] evsbin;
            public double[] evssplits;
            public dfinternalbuffers()
            {
                treebuf = new double[0];
                idxbuf = new int[0];
                tmpbufr = new double[0];
                tmpbufr2 = new double[0];
                tmpbufi = new int[0];
                classibuf = new int[0];
                sortrbuf = new double[0];
                sortrbuf2 = new double[0];
                sortibuf = new int[0];
                varpool = new int[0];
                evsbin = new bool[0];
                evssplits = new double[0];
            }
        };




        public const int innernodewidth = 3;
        public const int leafnodewidth = 2;
        public const int dfusestrongsplits = 1;
        public const int dfuseevs = 2;


        /*************************************************************************
        This subroutine builds random decision forest.

        INPUT PARAMETERS:
            XY          -   training set
            NPoints     -   training set size, NPoints>=1
            NVars       -   number of independent variables, NVars>=1
            NClasses    -   task type:
                            * NClasses=1 - regression task with one
                                           dependent variable
                            * NClasses>1 - classification task with
                                           NClasses classes.
            NTrees      -   number of trees in a forest, NTrees>=1.
                            recommended values: 50-100.
            R           -   percent of a training set used to build
                            individual trees. 0<R<=1.
                            recommended values: 0.1 <= R <= 0.66.

        OUTPUT PARAMETERS:
            Info        -   return code:
                            * -2, if there is a point with class number
                                  outside of [0..NClasses-1].
                            * -1, if incorrect parameters was passed
                                  (NPoints<1, NVars<1, NClasses<1, NTrees<1, R<=0
                                  or R>1).
                            *  1, if task has been solved
            DF          -   model built
            Rep         -   training report, contains error on a training set
                            and out-of-bag estimates of generalization error.

          -- ALGLIB --
             Copyright 19.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void dfbuildrandomdecisionforest(double[,] xy,
            int npoints,
            int nvars,
            int nclasses,
            int ntrees,
            double r,
            ref int info,
            decisionforest df,
            dfreport rep)
        {
            int samplesize = 0;

            info = 0;

            if( (double)(r)<=(double)(0) | (double)(r)>(double)(1) )
            {
                info = -1;
                return;
            }
            samplesize = Math.Max((int)Math.Round(r*npoints), 1);
            dfbuildinternal(xy, npoints, nvars, nclasses, ntrees, samplesize, Math.Max(nvars/2, 1), dfusestrongsplits+dfuseevs, ref info, df, rep);
        }


        public static void dfbuildinternal(double[,] xy,
            int npoints,
            int nvars,
            int nclasses,
            int ntrees,
            int samplesize,
            int nfeatures,
            int flags,
            ref int info,
            decisionforest df,
            dfreport rep)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int tmpi = 0;
            int lasttreeoffs = 0;
            int offs = 0;
            int ooboffs = 0;
            int treesize = 0;
            int nvarsinpool = 0;
            bool useevs = new bool();
            dfinternalbuffers bufs = new dfinternalbuffers();
            int[] permbuf = new int[0];
            double[] oobbuf = new double[0];
            int[] oobcntbuf = new int[0];
            double[,] xys = new double[0,0];
            double[] x = new double[0];
            double[] y = new double[0];
            int oobcnt = 0;
            int oobrelcnt = 0;
            double v = 0;
            double vmin = 0;
            double vmax = 0;
            bool bflag = new bool();
            int i_ = 0;
            int i1_ = 0;

            info = 0;

            
            //
            // Test for inputs
            //
            if( (((((npoints<1 | samplesize<1) | samplesize>npoints) | nvars<1) | nclasses<1) | ntrees<1) | nfeatures<1 )
            {
                info = -1;
                return;
            }
            if( nclasses>1 )
            {
                for(i=0; i<=npoints-1; i++)
                {
                    if( (int)Math.Round(xy[i,nvars])<0 | (int)Math.Round(xy[i,nvars])>=nclasses )
                    {
                        info = -2;
                        return;
                    }
                }
            }
            info = 1;
            
            //
            // Flags
            //
            useevs = flags/dfuseevs%2!=0;
            
            //
            // Allocate data, prepare header
            //
            treesize = 1+innernodewidth*(samplesize-1)+leafnodewidth*samplesize;
            permbuf = new int[npoints-1+1];
            bufs.treebuf = new double[treesize-1+1];
            bufs.idxbuf = new int[npoints-1+1];
            bufs.tmpbufr = new double[npoints-1+1];
            bufs.tmpbufr2 = new double[npoints-1+1];
            bufs.tmpbufi = new int[npoints-1+1];
            bufs.sortrbuf = new double[npoints];
            bufs.sortrbuf2 = new double[npoints];
            bufs.sortibuf = new int[npoints];
            bufs.varpool = new int[nvars-1+1];
            bufs.evsbin = new bool[nvars-1+1];
            bufs.evssplits = new double[nvars-1+1];
            bufs.classibuf = new int[2*nclasses-1+1];
            oobbuf = new double[nclasses*npoints-1+1];
            oobcntbuf = new int[npoints-1+1];
            df.trees = new double[ntrees*treesize-1+1];
            xys = new double[samplesize-1+1, nvars+1];
            x = new double[nvars-1+1];
            y = new double[nclasses-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                permbuf[i] = i;
            }
            for(i=0; i<=npoints*nclasses-1; i++)
            {
                oobbuf[i] = 0;
            }
            for(i=0; i<=npoints-1; i++)
            {
                oobcntbuf[i] = 0;
            }
            
            //
            // Prepare variable pool and EVS (extended variable selection/splitting) buffers
            // (whether EVS is turned on or not):
            // 1. detect binary variables and pre-calculate splits for them
            // 2. detect variables with non-distinct values and exclude them from pool
            //
            for(i=0; i<=nvars-1; i++)
            {
                bufs.varpool[i] = i;
            }
            nvarsinpool = nvars;
            if( useevs )
            {
                for(j=0; j<=nvars-1; j++)
                {
                    vmin = xy[0,j];
                    vmax = vmin;
                    for(i=0; i<=npoints-1; i++)
                    {
                        v = xy[i,j];
                        vmin = Math.Min(vmin, v);
                        vmax = Math.Max(vmax, v);
                    }
                    if( (double)(vmin)==(double)(vmax) )
                    {
                        
                        //
                        // exclude variable from pool
                        //
                        bufs.varpool[j] = bufs.varpool[nvarsinpool-1];
                        bufs.varpool[nvarsinpool-1] = -1;
                        nvarsinpool = nvarsinpool-1;
                        continue;
                    }
                    bflag = false;
                    for(i=0; i<=npoints-1; i++)
                    {
                        v = xy[i,j];
                        if( (double)(v)!=(double)(vmin) & (double)(v)!=(double)(vmax) )
                        {
                            bflag = true;
                            break;
                        }
                    }
                    if( bflag )
                    {
                        
                        //
                        // non-binary variable
                        //
                        bufs.evsbin[j] = false;
                    }
                    else
                    {
                        
                        //
                        // Prepare
                        //
                        bufs.evsbin[j] = true;
                        bufs.evssplits[j] = 0.5*(vmin+vmax);
                        if( (double)(bufs.evssplits[j])<=(double)(vmin) )
                        {
                            bufs.evssplits[j] = vmax;
                        }
                    }
                }
            }
            
            //
            // RANDOM FOREST FORMAT
            // W[0]         -   size of array
            // W[1]         -   version number
            // W[2]         -   NVars
            // W[3]         -   NClasses (1 for regression)
            // W[4]         -   NTrees
            // W[5]         -   trees offset
            //
            //
            // TREE FORMAT
            // W[Offs]      -   size of sub-array
            //     node info:
            // W[K+0]       -   variable number        (-1 for leaf mode)
            // W[K+1]       -   threshold              (class/value for leaf node)
            // W[K+2]       -   ">=" branch index      (absent for leaf node)
            //
            //
            df.nvars = nvars;
            df.nclasses = nclasses;
            df.ntrees = ntrees;
            
            //
            // Build forest
            //
            offs = 0;
            for(i=0; i<=ntrees-1; i++)
            {
                
                //
                // Prepare sample
                //
                for(k=0; k<=samplesize-1; k++)
                {
                    j = k+math.randominteger(npoints-k);
                    tmpi = permbuf[k];
                    permbuf[k] = permbuf[j];
                    permbuf[j] = tmpi;
                    j = permbuf[k];
                    for(i_=0; i_<=nvars;i_++)
                    {
                        xys[k,i_] = xy[j,i_];
                    }
                }
                
                //
                // build tree, copy
                //
                dfbuildtree(xys, samplesize, nvars, nclasses, nfeatures, nvarsinpool, flags, bufs);
                j = (int)Math.Round(bufs.treebuf[0]);
                i1_ = (0) - (offs);
                for(i_=offs; i_<=offs+j-1;i_++)
                {
                    df.trees[i_] = bufs.treebuf[i_+i1_];
                }
                lasttreeoffs = offs;
                offs = offs+j;
                
                //
                // OOB estimates
                //
                for(k=samplesize; k<=npoints-1; k++)
                {
                    for(j=0; j<=nclasses-1; j++)
                    {
                        y[j] = 0;
                    }
                    j = permbuf[k];
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        x[i_] = xy[j,i_];
                    }
                    dfprocessinternal(df, lasttreeoffs, x, ref y);
                    i1_ = (0) - (j*nclasses);
                    for(i_=j*nclasses; i_<=(j+1)*nclasses-1;i_++)
                    {
                        oobbuf[i_] = oobbuf[i_] + y[i_+i1_];
                    }
                    oobcntbuf[j] = oobcntbuf[j]+1;
                }
            }
            df.bufsize = offs;
            
            //
            // Normalize OOB results
            //
            for(i=0; i<=npoints-1; i++)
            {
                if( oobcntbuf[i]!=0 )
                {
                    v = (double)1/(double)oobcntbuf[i];
                    for(i_=i*nclasses; i_<=i*nclasses+nclasses-1;i_++)
                    {
                        oobbuf[i_] = v*oobbuf[i_];
                    }
                }
            }
            
            //
            // Calculate training set estimates
            //
            rep.relclserror = dfrelclserror(df, xy, npoints);
            rep.avgce = dfavgce(df, xy, npoints);
            rep.rmserror = dfrmserror(df, xy, npoints);
            rep.avgerror = dfavgerror(df, xy, npoints);
            rep.avgrelerror = dfavgrelerror(df, xy, npoints);
            
            //
            // Calculate OOB estimates.
            //
            rep.oobrelclserror = 0;
            rep.oobavgce = 0;
            rep.oobrmserror = 0;
            rep.oobavgerror = 0;
            rep.oobavgrelerror = 0;
            oobcnt = 0;
            oobrelcnt = 0;
            for(i=0; i<=npoints-1; i++)
            {
                if( oobcntbuf[i]!=0 )
                {
                    ooboffs = i*nclasses;
                    if( nclasses>1 )
                    {
                        
                        //
                        // classification-specific code
                        //
                        k = (int)Math.Round(xy[i,nvars]);
                        tmpi = 0;
                        for(j=1; j<=nclasses-1; j++)
                        {
                            if( (double)(oobbuf[ooboffs+j])>(double)(oobbuf[ooboffs+tmpi]) )
                            {
                                tmpi = j;
                            }
                        }
                        if( tmpi!=k )
                        {
                            rep.oobrelclserror = rep.oobrelclserror+1;
                        }
                        if( (double)(oobbuf[ooboffs+k])!=(double)(0) )
                        {
                            rep.oobavgce = rep.oobavgce-Math.Log(oobbuf[ooboffs+k]);
                        }
                        else
                        {
                            rep.oobavgce = rep.oobavgce-Math.Log(math.minrealnumber);
                        }
                        for(j=0; j<=nclasses-1; j++)
                        {
                            if( j==k )
                            {
                                rep.oobrmserror = rep.oobrmserror+math.sqr(oobbuf[ooboffs+j]-1);
                                rep.oobavgerror = rep.oobavgerror+Math.Abs(oobbuf[ooboffs+j]-1);
                                rep.oobavgrelerror = rep.oobavgrelerror+Math.Abs(oobbuf[ooboffs+j]-1);
                                oobrelcnt = oobrelcnt+1;
                            }
                            else
                            {
                                rep.oobrmserror = rep.oobrmserror+math.sqr(oobbuf[ooboffs+j]);
                                rep.oobavgerror = rep.oobavgerror+Math.Abs(oobbuf[ooboffs+j]);
                            }
                        }
                    }
                    else
                    {
                        
                        //
                        // regression-specific code
                        //
                        rep.oobrmserror = rep.oobrmserror+math.sqr(oobbuf[ooboffs]-xy[i,nvars]);
                        rep.oobavgerror = rep.oobavgerror+Math.Abs(oobbuf[ooboffs]-xy[i,nvars]);
                        if( (double)(xy[i,nvars])!=(double)(0) )
                        {
                            rep.oobavgrelerror = rep.oobavgrelerror+Math.Abs((oobbuf[ooboffs]-xy[i,nvars])/xy[i,nvars]);
                            oobrelcnt = oobrelcnt+1;
                        }
                    }
                    
                    //
                    // update OOB estimates count.
                    //
                    oobcnt = oobcnt+1;
                }
            }
            if( oobcnt>0 )
            {
                rep.oobrelclserror = rep.oobrelclserror/oobcnt;
                rep.oobavgce = rep.oobavgce/oobcnt;
                rep.oobrmserror = Math.Sqrt(rep.oobrmserror/(oobcnt*nclasses));
                rep.oobavgerror = rep.oobavgerror/(oobcnt*nclasses);
                if( oobrelcnt>0 )
                {
                    rep.oobavgrelerror = rep.oobavgrelerror/oobrelcnt;
                }
            }
        }


        /*************************************************************************
        Procesing

        INPUT PARAMETERS:
            DF      -   decision forest model
            X       -   input vector,  array[0..NVars-1].

        OUTPUT PARAMETERS:
            Y       -   result. Regression estimate when solving regression  task,
                        vector of posterior probabilities for classification task.

        See also DFProcessI.

          -- ALGLIB --
             Copyright 16.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void dfprocess(decisionforest df,
            double[] x,
            ref double[] y)
        {
            int offs = 0;
            int i = 0;
            double v = 0;
            int i_ = 0;

            
            //
            // Proceed
            //
            if( ap.len(y)<df.nclasses )
            {
                y = new double[df.nclasses];
            }
            offs = 0;
            for(i=0; i<=df.nclasses-1; i++)
            {
                y[i] = 0;
            }
            for(i=0; i<=df.ntrees-1; i++)
            {
                
                //
                // Process basic tree
                //
                dfprocessinternal(df, offs, x, ref y);
                
                //
                // Next tree
                //
                offs = offs+(int)Math.Round(df.trees[offs]);
            }
            v = (double)1/(double)df.ntrees;
            for(i_=0; i_<=df.nclasses-1;i_++)
            {
                y[i_] = v*y[i_];
            }
        }


        /*************************************************************************
        'interactive' variant of DFProcess for languages like Python which support
        constructs like "Y = DFProcessI(DF,X)" and interactive mode of interpreter

        This function allocates new array on each call,  so  it  is  significantly
        slower than its 'non-interactive' counterpart, but it is  more  convenient
        when you call it from command line.

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void dfprocessi(decisionforest df,
            double[] x,
            ref double[] y)
        {
            y = new double[0];

            dfprocess(df, x, ref y);
        }


        /*************************************************************************
        Relative classification error on the test set

        INPUT PARAMETERS:
            DF      -   decision forest model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            percent of incorrectly classified cases.
            Zero if model solves regression task.

          -- ALGLIB --
             Copyright 16.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double dfrelclserror(decisionforest df,
            double[,] xy,
            int npoints)
        {
            double result = 0;

            result = (double)dfclserror(df, xy, npoints)/(double)npoints;
            return result;
        }


        /*************************************************************************
        Average cross-entropy (in bits per element) on the test set

        INPUT PARAMETERS:
            DF      -   decision forest model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            CrossEntropy/(NPoints*LN(2)).
            Zero if model solves regression task.

          -- ALGLIB --
             Copyright 16.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double dfavgce(decisionforest df,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            double[] x = new double[0];
            double[] y = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            int tmpi = 0;
            int i_ = 0;

            x = new double[df.nvars-1+1];
            y = new double[df.nclasses-1+1];
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=df.nvars-1;i_++)
                {
                    x[i_] = xy[i,i_];
                }
                dfprocess(df, x, ref y);
                if( df.nclasses>1 )
                {
                    
                    //
                    // classification-specific code
                    //
                    k = (int)Math.Round(xy[i,df.nvars]);
                    tmpi = 0;
                    for(j=1; j<=df.nclasses-1; j++)
                    {
                        if( (double)(y[j])>(double)(y[tmpi]) )
                        {
                            tmpi = j;
                        }
                    }
                    if( (double)(y[k])!=(double)(0) )
                    {
                        result = result-Math.Log(y[k]);
                    }
                    else
                    {
                        result = result-Math.Log(math.minrealnumber);
                    }
                }
            }
            result = result/npoints;
            return result;
        }


        /*************************************************************************
        RMS error on the test set

        INPUT PARAMETERS:
            DF      -   decision forest model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            root mean square error.
            Its meaning for regression task is obvious. As for
            classification task, RMS error means error when estimating posterior
            probabilities.

          -- ALGLIB --
             Copyright 16.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double dfrmserror(decisionforest df,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            double[] x = new double[0];
            double[] y = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            int tmpi = 0;
            int i_ = 0;

            x = new double[df.nvars-1+1];
            y = new double[df.nclasses-1+1];
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=df.nvars-1;i_++)
                {
                    x[i_] = xy[i,i_];
                }
                dfprocess(df, x, ref y);
                if( df.nclasses>1 )
                {
                    
                    //
                    // classification-specific code
                    //
                    k = (int)Math.Round(xy[i,df.nvars]);
                    tmpi = 0;
                    for(j=1; j<=df.nclasses-1; j++)
                    {
                        if( (double)(y[j])>(double)(y[tmpi]) )
                        {
                            tmpi = j;
                        }
                    }
                    for(j=0; j<=df.nclasses-1; j++)
                    {
                        if( j==k )
                        {
                            result = result+math.sqr(y[j]-1);
                        }
                        else
                        {
                            result = result+math.sqr(y[j]);
                        }
                    }
                }
                else
                {
                    
                    //
                    // regression-specific code
                    //
                    result = result+math.sqr(y[0]-xy[i,df.nvars]);
                }
            }
            result = Math.Sqrt(result/(npoints*df.nclasses));
            return result;
        }


        /*************************************************************************
        Average error on the test set

        INPUT PARAMETERS:
            DF      -   decision forest model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            Its meaning for regression task is obvious. As for
            classification task, it means average error when estimating posterior
            probabilities.

          -- ALGLIB --
             Copyright 16.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double dfavgerror(decisionforest df,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            double[] x = new double[0];
            double[] y = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            int i_ = 0;

            x = new double[df.nvars-1+1];
            y = new double[df.nclasses-1+1];
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=df.nvars-1;i_++)
                {
                    x[i_] = xy[i,i_];
                }
                dfprocess(df, x, ref y);
                if( df.nclasses>1 )
                {
                    
                    //
                    // classification-specific code
                    //
                    k = (int)Math.Round(xy[i,df.nvars]);
                    for(j=0; j<=df.nclasses-1; j++)
                    {
                        if( j==k )
                        {
                            result = result+Math.Abs(y[j]-1);
                        }
                        else
                        {
                            result = result+Math.Abs(y[j]);
                        }
                    }
                }
                else
                {
                    
                    //
                    // regression-specific code
                    //
                    result = result+Math.Abs(y[0]-xy[i,df.nvars]);
                }
            }
            result = result/(npoints*df.nclasses);
            return result;
        }


        /*************************************************************************
        Average relative error on the test set

        INPUT PARAMETERS:
            DF      -   decision forest model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            Its meaning for regression task is obvious. As for
            classification task, it means average relative error when estimating
            posterior probability of belonging to the correct class.

          -- ALGLIB --
             Copyright 16.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double dfavgrelerror(decisionforest df,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            double[] x = new double[0];
            double[] y = new double[0];
            int relcnt = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int i_ = 0;

            x = new double[df.nvars-1+1];
            y = new double[df.nclasses-1+1];
            result = 0;
            relcnt = 0;
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=df.nvars-1;i_++)
                {
                    x[i_] = xy[i,i_];
                }
                dfprocess(df, x, ref y);
                if( df.nclasses>1 )
                {
                    
                    //
                    // classification-specific code
                    //
                    k = (int)Math.Round(xy[i,df.nvars]);
                    for(j=0; j<=df.nclasses-1; j++)
                    {
                        if( j==k )
                        {
                            result = result+Math.Abs(y[j]-1);
                            relcnt = relcnt+1;
                        }
                    }
                }
                else
                {
                    
                    //
                    // regression-specific code
                    //
                    if( (double)(xy[i,df.nvars])!=(double)(0) )
                    {
                        result = result+Math.Abs((y[0]-xy[i,df.nvars])/xy[i,df.nvars]);
                        relcnt = relcnt+1;
                    }
                }
            }
            if( relcnt>0 )
            {
                result = result/relcnt;
            }
            return result;
        }


        /*************************************************************************
        Copying of DecisionForest strucure

        INPUT PARAMETERS:
            DF1 -   original

        OUTPUT PARAMETERS:
            DF2 -   copy

          -- ALGLIB --
             Copyright 13.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void dfcopy(decisionforest df1,
            decisionforest df2)
        {
            int i_ = 0;

            df2.nvars = df1.nvars;
            df2.nclasses = df1.nclasses;
            df2.ntrees = df1.ntrees;
            df2.bufsize = df1.bufsize;
            df2.trees = new double[df1.bufsize-1+1];
            for(i_=0; i_<=df1.bufsize-1;i_++)
            {
                df2.trees[i_] = df1.trees[i_];
            }
        }


        /*************************************************************************
        Classification error
        *************************************************************************/
        private static int dfclserror(decisionforest df,
            double[,] xy,
            int npoints)
        {
            int result = 0;
            double[] x = new double[0];
            double[] y = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            int tmpi = 0;
            int i_ = 0;

            if( df.nclasses<=1 )
            {
                result = 0;
                return result;
            }
            x = new double[df.nvars-1+1];
            y = new double[df.nclasses-1+1];
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=df.nvars-1;i_++)
                {
                    x[i_] = xy[i,i_];
                }
                dfprocess(df, x, ref y);
                k = (int)Math.Round(xy[i,df.nvars]);
                tmpi = 0;
                for(j=1; j<=df.nclasses-1; j++)
                {
                    if( (double)(y[j])>(double)(y[tmpi]) )
                    {
                        tmpi = j;
                    }
                }
                if( tmpi!=k )
                {
                    result = result+1;
                }
            }
            return result;
        }


        /*************************************************************************
        Internal subroutine for processing one decision tree starting at Offs
        *************************************************************************/
        private static void dfprocessinternal(decisionforest df,
            int offs,
            double[] x,
            ref double[] y)
        {
            int k = 0;
            int idx = 0;

            
            //
            // Set pointer to the root
            //
            k = offs+1;
            
            //
            // Navigate through the tree
            //
            while( true )
            {
                if( (double)(df.trees[k])==(double)(-1) )
                {
                    if( df.nclasses==1 )
                    {
                        y[0] = y[0]+df.trees[k+1];
                    }
                    else
                    {
                        idx = (int)Math.Round(df.trees[k+1]);
                        y[idx] = y[idx]+1;
                    }
                    break;
                }
                if( (double)(x[(int)Math.Round(df.trees[k])])<(double)(df.trees[k+1]) )
                {
                    k = k+innernodewidth;
                }
                else
                {
                    k = offs+(int)Math.Round(df.trees[k+2]);
                }
            }
        }


        /*************************************************************************
        Builds one decision tree. Just a wrapper for the DFBuildTreeRec.
        *************************************************************************/
        private static void dfbuildtree(double[,] xy,
            int npoints,
            int nvars,
            int nclasses,
            int nfeatures,
            int nvarsinpool,
            int flags,
            dfinternalbuffers bufs)
        {
            int numprocessed = 0;
            int i = 0;

            ap.assert(npoints>0);
            
            //
            // Prepare IdxBuf. It stores indices of the training set elements.
            // When training set is being split, contents of IdxBuf is
            // correspondingly reordered so we can know which elements belong
            // to which branch of decision tree.
            //
            for(i=0; i<=npoints-1; i++)
            {
                bufs.idxbuf[i] = i;
            }
            
            //
            // Recursive procedure
            //
            numprocessed = 1;
            dfbuildtreerec(xy, npoints, nvars, nclasses, nfeatures, nvarsinpool, flags, ref numprocessed, 0, npoints-1, bufs);
            bufs.treebuf[0] = numprocessed;
        }


        /*************************************************************************
        Builds one decision tree (internal recursive subroutine)

        Parameters:
            TreeBuf     -   large enough array, at least TreeSize
            IdxBuf      -   at least NPoints elements
            TmpBufR     -   at least NPoints
            TmpBufR2    -   at least NPoints
            TmpBufI     -   at least NPoints
            TmpBufI2    -   at least NPoints+1
        *************************************************************************/
        private static void dfbuildtreerec(double[,] xy,
            int npoints,
            int nvars,
            int nclasses,
            int nfeatures,
            int nvarsinpool,
            int flags,
            ref int numprocessed,
            int idx1,
            int idx2,
            dfinternalbuffers bufs)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            bool bflag = new bool();
            int i1 = 0;
            int i2 = 0;
            int info = 0;
            double sl = 0;
            double sr = 0;
            double w = 0;
            int idxbest = 0;
            double ebest = 0;
            double tbest = 0;
            int varcur = 0;
            double s = 0;
            double v = 0;
            double v1 = 0;
            double v2 = 0;
            double threshold = 0;
            int oldnp = 0;
            double currms = 0;
            bool useevs = new bool();

            
            //
            // these initializers are not really necessary,
            // but without them compiler complains about uninitialized locals
            //
            tbest = 0;
            
            //
            // Prepare
            //
            ap.assert(npoints>0);
            ap.assert(idx2>=idx1);
            useevs = flags/dfuseevs%2!=0;
            
            //
            // Leaf node
            //
            if( idx2==idx1 )
            {
                bufs.treebuf[numprocessed] = -1;
                bufs.treebuf[numprocessed+1] = xy[bufs.idxbuf[idx1],nvars];
                numprocessed = numprocessed+leafnodewidth;
                return;
            }
            
            //
            // Non-leaf node.
            // Select random variable, prepare split:
            // 1. prepare default solution - no splitting, class at random
            // 2. investigate possible splits, compare with default/best
            //
            idxbest = -1;
            if( nclasses>1 )
            {
                
                //
                // default solution for classification
                //
                for(i=0; i<=nclasses-1; i++)
                {
                    bufs.classibuf[i] = 0;
                }
                s = idx2-idx1+1;
                for(i=idx1; i<=idx2; i++)
                {
                    j = (int)Math.Round(xy[bufs.idxbuf[i],nvars]);
                    bufs.classibuf[j] = bufs.classibuf[j]+1;
                }
                ebest = 0;
                for(i=0; i<=nclasses-1; i++)
                {
                    ebest = ebest+bufs.classibuf[i]*math.sqr(1-bufs.classibuf[i]/s)+(s-bufs.classibuf[i])*math.sqr(bufs.classibuf[i]/s);
                }
                ebest = Math.Sqrt(ebest/(nclasses*(idx2-idx1+1)));
            }
            else
            {
                
                //
                // default solution for regression
                //
                v = 0;
                for(i=idx1; i<=idx2; i++)
                {
                    v = v+xy[bufs.idxbuf[i],nvars];
                }
                v = v/(idx2-idx1+1);
                ebest = 0;
                for(i=idx1; i<=idx2; i++)
                {
                    ebest = ebest+math.sqr(xy[bufs.idxbuf[i],nvars]-v);
                }
                ebest = Math.Sqrt(ebest/(idx2-idx1+1));
            }
            i = 0;
            while( i<=Math.Min(nfeatures, nvarsinpool)-1 )
            {
                
                //
                // select variables from pool
                //
                j = i+math.randominteger(nvarsinpool-i);
                k = bufs.varpool[i];
                bufs.varpool[i] = bufs.varpool[j];
                bufs.varpool[j] = k;
                varcur = bufs.varpool[i];
                
                //
                // load variable values to working array
                //
                // apply EVS preprocessing: if all variable values are same,
                // variable is excluded from pool.
                //
                // This is necessary for binary pre-splits (see later) to work.
                //
                for(j=idx1; j<=idx2; j++)
                {
                    bufs.tmpbufr[j-idx1] = xy[bufs.idxbuf[j],varcur];
                }
                if( useevs )
                {
                    bflag = false;
                    v = bufs.tmpbufr[0];
                    for(j=0; j<=idx2-idx1; j++)
                    {
                        if( (double)(bufs.tmpbufr[j])!=(double)(v) )
                        {
                            bflag = true;
                            break;
                        }
                    }
                    if( !bflag )
                    {
                        
                        //
                        // exclude variable from pool,
                        // go to the next iteration.
                        // I is not increased.
                        //
                        k = bufs.varpool[i];
                        bufs.varpool[i] = bufs.varpool[nvarsinpool-1];
                        bufs.varpool[nvarsinpool-1] = k;
                        nvarsinpool = nvarsinpool-1;
                        continue;
                    }
                }
                
                //
                // load labels to working array
                //
                if( nclasses>1 )
                {
                    for(j=idx1; j<=idx2; j++)
                    {
                        bufs.tmpbufi[j-idx1] = (int)Math.Round(xy[bufs.idxbuf[j],nvars]);
                    }
                }
                else
                {
                    for(j=idx1; j<=idx2; j++)
                    {
                        bufs.tmpbufr2[j-idx1] = xy[bufs.idxbuf[j],nvars];
                    }
                }
                
                //
                // calculate split
                //
                if( useevs & bufs.evsbin[varcur] )
                {
                    
                    //
                    // Pre-calculated splits for binary variables.
                    // Threshold is already known, just calculate RMS error
                    //
                    threshold = bufs.evssplits[varcur];
                    if( nclasses>1 )
                    {
                        
                        //
                        // classification-specific code
                        //
                        for(j=0; j<=2*nclasses-1; j++)
                        {
                            bufs.classibuf[j] = 0;
                        }
                        sl = 0;
                        sr = 0;
                        for(j=0; j<=idx2-idx1; j++)
                        {
                            k = bufs.tmpbufi[j];
                            if( (double)(bufs.tmpbufr[j])<(double)(threshold) )
                            {
                                bufs.classibuf[k] = bufs.classibuf[k]+1;
                                sl = sl+1;
                            }
                            else
                            {
                                bufs.classibuf[k+nclasses] = bufs.classibuf[k+nclasses]+1;
                                sr = sr+1;
                            }
                        }
                        ap.assert((double)(sl)!=(double)(0) & (double)(sr)!=(double)(0), "DFBuildTreeRec: something strange!");
                        currms = 0;
                        for(j=0; j<=nclasses-1; j++)
                        {
                            w = bufs.classibuf[j];
                            currms = currms+w*math.sqr(w/sl-1);
                            currms = currms+(sl-w)*math.sqr(w/sl);
                            w = bufs.classibuf[nclasses+j];
                            currms = currms+w*math.sqr(w/sr-1);
                            currms = currms+(sr-w)*math.sqr(w/sr);
                        }
                        currms = Math.Sqrt(currms/(nclasses*(idx2-idx1+1)));
                    }
                    else
                    {
                        
                        //
                        // regression-specific code
                        //
                        sl = 0;
                        sr = 0;
                        v1 = 0;
                        v2 = 0;
                        for(j=0; j<=idx2-idx1; j++)
                        {
                            if( (double)(bufs.tmpbufr[j])<(double)(threshold) )
                            {
                                v1 = v1+bufs.tmpbufr2[j];
                                sl = sl+1;
                            }
                            else
                            {
                                v2 = v2+bufs.tmpbufr2[j];
                                sr = sr+1;
                            }
                        }
                        ap.assert((double)(sl)!=(double)(0) & (double)(sr)!=(double)(0), "DFBuildTreeRec: something strange!");
                        v1 = v1/sl;
                        v2 = v2/sr;
                        currms = 0;
                        for(j=0; j<=idx2-idx1; j++)
                        {
                            if( (double)(bufs.tmpbufr[j])<(double)(threshold) )
                            {
                                currms = currms+math.sqr(v1-bufs.tmpbufr2[j]);
                            }
                            else
                            {
                                currms = currms+math.sqr(v2-bufs.tmpbufr2[j]);
                            }
                        }
                        currms = Math.Sqrt(currms/(idx2-idx1+1));
                    }
                    info = 1;
                }
                else
                {
                    
                    //
                    // Generic splits
                    //
                    if( nclasses>1 )
                    {
                        dfsplitc(ref bufs.tmpbufr, ref bufs.tmpbufi, ref bufs.classibuf, idx2-idx1+1, nclasses, dfusestrongsplits, ref info, ref threshold, ref currms, ref bufs.sortrbuf, ref bufs.sortibuf);
                    }
                    else
                    {
                        dfsplitr(ref bufs.tmpbufr, ref bufs.tmpbufr2, idx2-idx1+1, dfusestrongsplits, ref info, ref threshold, ref currms, ref bufs.sortrbuf, ref bufs.sortrbuf2);
                    }
                }
                if( info>0 )
                {
                    if( (double)(currms)<=(double)(ebest) )
                    {
                        ebest = currms;
                        idxbest = varcur;
                        tbest = threshold;
                    }
                }
                
                //
                // Next iteration
                //
                i = i+1;
            }
            
            //
            // to split or not to split
            //
            if( idxbest<0 )
            {
                
                //
                // All values are same, cannot split.
                //
                bufs.treebuf[numprocessed] = -1;
                if( nclasses>1 )
                {
                    
                    //
                    // Select random class label (randomness allows us to
                    // approximate distribution of the classes)
                    //
                    bufs.treebuf[numprocessed+1] = (int)Math.Round(xy[bufs.idxbuf[idx1+math.randominteger(idx2-idx1+1)],nvars]);
                }
                else
                {
                    
                    //
                    // Select average (for regression task).
                    //
                    v = 0;
                    for(i=idx1; i<=idx2; i++)
                    {
                        v = v+xy[bufs.idxbuf[i],nvars]/(idx2-idx1+1);
                    }
                    bufs.treebuf[numprocessed+1] = v;
                }
                numprocessed = numprocessed+leafnodewidth;
            }
            else
            {
                
                //
                // we can split
                //
                bufs.treebuf[numprocessed] = idxbest;
                bufs.treebuf[numprocessed+1] = tbest;
                i1 = idx1;
                i2 = idx2;
                while( i1<=i2 )
                {
                    
                    //
                    // Reorder indices so that left partition is in [Idx1..I1-1],
                    // and right partition is in [I2+1..Idx2]
                    //
                    if( (double)(xy[bufs.idxbuf[i1],idxbest])<(double)(tbest) )
                    {
                        i1 = i1+1;
                        continue;
                    }
                    if( (double)(xy[bufs.idxbuf[i2],idxbest])>=(double)(tbest) )
                    {
                        i2 = i2-1;
                        continue;
                    }
                    j = bufs.idxbuf[i1];
                    bufs.idxbuf[i1] = bufs.idxbuf[i2];
                    bufs.idxbuf[i2] = j;
                    i1 = i1+1;
                    i2 = i2-1;
                }
                oldnp = numprocessed;
                numprocessed = numprocessed+innernodewidth;
                dfbuildtreerec(xy, npoints, nvars, nclasses, nfeatures, nvarsinpool, flags, ref numprocessed, idx1, i1-1, bufs);
                bufs.treebuf[oldnp+2] = numprocessed;
                dfbuildtreerec(xy, npoints, nvars, nclasses, nfeatures, nvarsinpool, flags, ref numprocessed, i2+1, idx2, bufs);
            }
        }


        /*************************************************************************
        Makes split on attribute
        *************************************************************************/
        private static void dfsplitc(ref double[] x,
            ref int[] c,
            ref int[] cntbuf,
            int n,
            int nc,
            int flags,
            ref int info,
            ref double threshold,
            ref double e,
            ref double[] sortrbuf,
            ref int[] sortibuf)
        {
            int i = 0;
            int neq = 0;
            int nless = 0;
            int ngreater = 0;
            int q = 0;
            int qmin = 0;
            int qmax = 0;
            int qcnt = 0;
            double cursplit = 0;
            int nleft = 0;
            double v = 0;
            double cure = 0;
            double w = 0;
            double sl = 0;
            double sr = 0;

            info = 0;
            threshold = 0;
            e = 0;

            tsort.tagsortfasti(ref x, ref c, ref sortrbuf, ref sortibuf, n);
            e = math.maxrealnumber;
            threshold = 0.5*(x[0]+x[n-1]);
            info = -3;
            if( flags/dfusestrongsplits%2==0 )
            {
                
                //
                // weak splits, split at half
                //
                qcnt = 2;
                qmin = 1;
                qmax = 1;
            }
            else
            {
                
                //
                // strong splits: choose best quartile
                //
                qcnt = 4;
                qmin = 1;
                qmax = 3;
            }
            for(q=qmin; q<=qmax; q++)
            {
                cursplit = x[n*q/qcnt];
                neq = 0;
                nless = 0;
                ngreater = 0;
                for(i=0; i<=n-1; i++)
                {
                    if( (double)(x[i])<(double)(cursplit) )
                    {
                        nless = nless+1;
                    }
                    if( (double)(x[i])==(double)(cursplit) )
                    {
                        neq = neq+1;
                    }
                    if( (double)(x[i])>(double)(cursplit) )
                    {
                        ngreater = ngreater+1;
                    }
                }
                ap.assert(neq!=0, "DFSplitR: NEq=0, something strange!!!");
                if( nless!=0 | ngreater!=0 )
                {
                    
                    //
                    // set threshold between two partitions, with
                    // some tweaking to avoid problems with floating point
                    // arithmetics.
                    //
                    // The problem is that when you calculates C = 0.5*(A+B) there
                    // can be no C which lies strictly between A and B (for example,
                    // there is no floating point number which is
                    // greater than 1 and less than 1+eps). In such situations
                    // we choose right side as theshold (remember that
                    // points which lie on threshold falls to the right side).
                    //
                    if( nless<ngreater )
                    {
                        cursplit = 0.5*(x[nless+neq-1]+x[nless+neq]);
                        nleft = nless+neq;
                        if( (double)(cursplit)<=(double)(x[nless+neq-1]) )
                        {
                            cursplit = x[nless+neq];
                        }
                    }
                    else
                    {
                        cursplit = 0.5*(x[nless-1]+x[nless]);
                        nleft = nless;
                        if( (double)(cursplit)<=(double)(x[nless-1]) )
                        {
                            cursplit = x[nless];
                        }
                    }
                    info = 1;
                    cure = 0;
                    for(i=0; i<=2*nc-1; i++)
                    {
                        cntbuf[i] = 0;
                    }
                    for(i=0; i<=nleft-1; i++)
                    {
                        cntbuf[c[i]] = cntbuf[c[i]]+1;
                    }
                    for(i=nleft; i<=n-1; i++)
                    {
                        cntbuf[nc+c[i]] = cntbuf[nc+c[i]]+1;
                    }
                    sl = nleft;
                    sr = n-nleft;
                    v = 0;
                    for(i=0; i<=nc-1; i++)
                    {
                        w = cntbuf[i];
                        v = v+w*math.sqr(w/sl-1);
                        v = v+(sl-w)*math.sqr(w/sl);
                        w = cntbuf[nc+i];
                        v = v+w*math.sqr(w/sr-1);
                        v = v+(sr-w)*math.sqr(w/sr);
                    }
                    cure = Math.Sqrt(v/(nc*n));
                    if( (double)(cure)<(double)(e) )
                    {
                        threshold = cursplit;
                        e = cure;
                    }
                }
            }
        }


        /*************************************************************************
        Makes split on attribute
        *************************************************************************/
        private static void dfsplitr(ref double[] x,
            ref double[] y,
            int n,
            int flags,
            ref int info,
            ref double threshold,
            ref double e,
            ref double[] sortrbuf,
            ref double[] sortrbuf2)
        {
            int i = 0;
            int neq = 0;
            int nless = 0;
            int ngreater = 0;
            int q = 0;
            int qmin = 0;
            int qmax = 0;
            int qcnt = 0;
            double cursplit = 0;
            int nleft = 0;
            double v = 0;
            double cure = 0;

            info = 0;
            threshold = 0;
            e = 0;

            tsort.tagsortfastr(ref x, ref y, ref sortrbuf, ref sortrbuf2, n);
            e = math.maxrealnumber;
            threshold = 0.5*(x[0]+x[n-1]);
            info = -3;
            if( flags/dfusestrongsplits%2==0 )
            {
                
                //
                // weak splits, split at half
                //
                qcnt = 2;
                qmin = 1;
                qmax = 1;
            }
            else
            {
                
                //
                // strong splits: choose best quartile
                //
                qcnt = 4;
                qmin = 1;
                qmax = 3;
            }
            for(q=qmin; q<=qmax; q++)
            {
                cursplit = x[n*q/qcnt];
                neq = 0;
                nless = 0;
                ngreater = 0;
                for(i=0; i<=n-1; i++)
                {
                    if( (double)(x[i])<(double)(cursplit) )
                    {
                        nless = nless+1;
                    }
                    if( (double)(x[i])==(double)(cursplit) )
                    {
                        neq = neq+1;
                    }
                    if( (double)(x[i])>(double)(cursplit) )
                    {
                        ngreater = ngreater+1;
                    }
                }
                ap.assert(neq!=0, "DFSplitR: NEq=0, something strange!!!");
                if( nless!=0 | ngreater!=0 )
                {
                    
                    //
                    // set threshold between two partitions, with
                    // some tweaking to avoid problems with floating point
                    // arithmetics.
                    //
                    // The problem is that when you calculates C = 0.5*(A+B) there
                    // can be no C which lies strictly between A and B (for example,
                    // there is no floating point number which is
                    // greater than 1 and less than 1+eps). In such situations
                    // we choose right side as theshold (remember that
                    // points which lie on threshold falls to the right side).
                    //
                    if( nless<ngreater )
                    {
                        cursplit = 0.5*(x[nless+neq-1]+x[nless+neq]);
                        nleft = nless+neq;
                        if( (double)(cursplit)<=(double)(x[nless+neq-1]) )
                        {
                            cursplit = x[nless+neq];
                        }
                    }
                    else
                    {
                        cursplit = 0.5*(x[nless-1]+x[nless]);
                        nleft = nless;
                        if( (double)(cursplit)<=(double)(x[nless-1]) )
                        {
                            cursplit = x[nless];
                        }
                    }
                    info = 1;
                    cure = 0;
                    v = 0;
                    for(i=0; i<=nleft-1; i++)
                    {
                        v = v+y[i];
                    }
                    v = v/nleft;
                    for(i=0; i<=nleft-1; i++)
                    {
                        cure = cure+math.sqr(y[i]-v);
                    }
                    v = 0;
                    for(i=nleft; i<=n-1; i++)
                    {
                        v = v+y[i];
                    }
                    v = v/(n-nleft);
                    for(i=nleft; i<=n-1; i++)
                    {
                        cure = cure+math.sqr(y[i]-v);
                    }
                    cure = Math.Sqrt(cure/n);
                    if( (double)(cure)<(double)(e) )
                    {
                        threshold = cursplit;
                        e = cure;
                    }
                }
            }
        }


    }
    public class kmeans
    {
        /*************************************************************************
        k-means++ clusterization

        INPUT PARAMETERS:
            XY          -   dataset, array [0..NPoints-1,0..NVars-1].
            NPoints     -   dataset size, NPoints>=K
            NVars       -   number of variables, NVars>=1
            K           -   desired number of clusters, K>=1
            Restarts    -   number of restarts, Restarts>=1

        OUTPUT PARAMETERS:
            Info        -   return code:
                            * -3, if task is degenerate (number of distinct points is
                                  less than K)
                            * -1, if incorrect NPoints/NFeatures/K/Restarts was passed
                            *  1, if subroutine finished successfully
            C           -   array[0..NVars-1,0..K-1].matrix whose columns store
                            cluster's centers
            XYC         -   array which contains number of clusters dataset points
                            belong to.

          -- ALGLIB --
             Copyright 21.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void kmeansgenerate(double[,] xy,
            int npoints,
            int nvars,
            int k,
            int restarts,
            ref int info,
            ref double[,] c,
            ref int[] xyc)
        {
            int i = 0;
            int j = 0;
            double[,] ct = new double[0,0];
            double[,] ctbest = new double[0,0];
            int[] xycbest = new int[0];
            double e = 0;
            double ebest = 0;
            double[] x = new double[0];
            double[] tmp = new double[0];
            double[] d2 = new double[0];
            double[] p = new double[0];
            int[] csizes = new int[0];
            bool[] cbusy = new bool[0];
            double v = 0;
            int cclosest = 0;
            double dclosest = 0;
            double[] work = new double[0];
            bool waschanges = new bool();
            bool zerosizeclusters = new bool();
            int pass = 0;
            int i_ = 0;

            info = 0;
            c = new double[0,0];
            xyc = new int[0];

            
            //
            // Test parameters
            //
            if( ((npoints<k | nvars<1) | k<1) | restarts<1 )
            {
                info = -1;
                return;
            }
            
            //
            // TODO: special case K=1
            // TODO: special case K=NPoints
            //
            info = 1;
            
            //
            // Multiple passes of k-means++ algorithm
            //
            ct = new double[k, nvars];
            ctbest = new double[k, nvars];
            xyc = new int[npoints];
            xycbest = new int[npoints];
            d2 = new double[npoints];
            p = new double[npoints];
            tmp = new double[nvars];
            csizes = new int[k];
            cbusy = new bool[k];
            ebest = math.maxrealnumber;
            for(pass=1; pass<=restarts; pass++)
            {
                
                //
                // Select initial centers  using k-means++ algorithm
                // 1. Choose first center at random
                // 2. Choose next centers using their distance from centers already chosen
                //
                // Note that for performance reasons centers are stored in ROWS of CT, not
                // in columns. We'll transpose CT in the end and store it in the C.
                //
                i = math.randominteger(npoints);
                for(i_=0; i_<=nvars-1;i_++)
                {
                    ct[0,i_] = xy[i,i_];
                }
                cbusy[0] = true;
                for(i=1; i<=k-1; i++)
                {
                    cbusy[i] = false;
                }
                if( !selectcenterpp(xy, npoints, nvars, ref ct, cbusy, k, ref d2, ref p, ref tmp) )
                {
                    info = -3;
                    return;
                }
                
                //
                // Update centers:
                // 2. update center positions
                //
                for(i=0; i<=npoints-1; i++)
                {
                    xyc[i] = -1;
                }
                while( true )
                {
                    
                    //
                    // fill XYC with center numbers
                    //
                    waschanges = false;
                    for(i=0; i<=npoints-1; i++)
                    {
                        cclosest = -1;
                        dclosest = math.maxrealnumber;
                        for(j=0; j<=k-1; j++)
                        {
                            for(i_=0; i_<=nvars-1;i_++)
                            {
                                tmp[i_] = xy[i,i_];
                            }
                            for(i_=0; i_<=nvars-1;i_++)
                            {
                                tmp[i_] = tmp[i_] - ct[j,i_];
                            }
                            v = 0.0;
                            for(i_=0; i_<=nvars-1;i_++)
                            {
                                v += tmp[i_]*tmp[i_];
                            }
                            if( (double)(v)<(double)(dclosest) )
                            {
                                cclosest = j;
                                dclosest = v;
                            }
                        }
                        if( xyc[i]!=cclosest )
                        {
                            waschanges = true;
                        }
                        xyc[i] = cclosest;
                    }
                    
                    //
                    // Update centers
                    //
                    for(j=0; j<=k-1; j++)
                    {
                        csizes[j] = 0;
                    }
                    for(i=0; i<=k-1; i++)
                    {
                        for(j=0; j<=nvars-1; j++)
                        {
                            ct[i,j] = 0;
                        }
                    }
                    for(i=0; i<=npoints-1; i++)
                    {
                        csizes[xyc[i]] = csizes[xyc[i]]+1;
                        for(i_=0; i_<=nvars-1;i_++)
                        {
                            ct[xyc[i],i_] = ct[xyc[i],i_] + xy[i,i_];
                        }
                    }
                    zerosizeclusters = false;
                    for(i=0; i<=k-1; i++)
                    {
                        cbusy[i] = csizes[i]!=0;
                        zerosizeclusters = zerosizeclusters | csizes[i]==0;
                    }
                    if( zerosizeclusters )
                    {
                        
                        //
                        // Some clusters have zero size - rare, but possible.
                        // We'll choose new centers for such clusters using k-means++ rule
                        // and restart algorithm
                        //
                        if( !selectcenterpp(xy, npoints, nvars, ref ct, cbusy, k, ref d2, ref p, ref tmp) )
                        {
                            info = -3;
                            return;
                        }
                        continue;
                    }
                    for(j=0; j<=k-1; j++)
                    {
                        v = (double)1/(double)csizes[j];
                        for(i_=0; i_<=nvars-1;i_++)
                        {
                            ct[j,i_] = v*ct[j,i_];
                        }
                    }
                    
                    //
                    // if nothing has changed during iteration
                    //
                    if( !waschanges )
                    {
                        break;
                    }
                }
                
                //
                // 3. Calculate E, compare with best centers found so far
                //
                e = 0;
                for(i=0; i<=npoints-1; i++)
                {
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        tmp[i_] = xy[i,i_];
                    }
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        tmp[i_] = tmp[i_] - ct[xyc[i],i_];
                    }
                    v = 0.0;
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        v += tmp[i_]*tmp[i_];
                    }
                    e = e+v;
                }
                if( (double)(e)<(double)(ebest) )
                {
                    
                    //
                    // store partition.
                    //
                    ebest = e;
                    blas.copymatrix(ct, 0, k-1, 0, nvars-1, ref ctbest, 0, k-1, 0, nvars-1);
                    for(i=0; i<=npoints-1; i++)
                    {
                        xycbest[i] = xyc[i];
                    }
                }
            }
            
            //
            // Copy and transpose
            //
            c = new double[nvars-1+1, k-1+1];
            blas.copyandtranspose(ctbest, 0, k-1, 0, nvars-1, ref c, 0, nvars-1, 0, k-1);
            for(i=0; i<=npoints-1; i++)
            {
                xyc[i] = xycbest[i];
            }
        }


        /*************************************************************************
        Select center for a new cluster using k-means++ rule
        *************************************************************************/
        private static bool selectcenterpp(double[,] xy,
            int npoints,
            int nvars,
            ref double[,] centers,
            bool[] busycenters,
            int ccnt,
            ref double[] d2,
            ref double[] p,
            ref double[] tmp)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;
            int cc = 0;
            double v = 0;
            double s = 0;
            int i_ = 0;

            busycenters = (bool[])busycenters.Clone();

            result = true;
            for(cc=0; cc<=ccnt-1; cc++)
            {
                if( !busycenters[cc] )
                {
                    
                    //
                    // fill D2
                    //
                    for(i=0; i<=npoints-1; i++)
                    {
                        d2[i] = math.maxrealnumber;
                        for(j=0; j<=ccnt-1; j++)
                        {
                            if( busycenters[j] )
                            {
                                for(i_=0; i_<=nvars-1;i_++)
                                {
                                    tmp[i_] = xy[i,i_];
                                }
                                for(i_=0; i_<=nvars-1;i_++)
                                {
                                    tmp[i_] = tmp[i_] - centers[j,i_];
                                }
                                v = 0.0;
                                for(i_=0; i_<=nvars-1;i_++)
                                {
                                    v += tmp[i_]*tmp[i_];
                                }
                                if( (double)(v)<(double)(d2[i]) )
                                {
                                    d2[i] = v;
                                }
                            }
                        }
                    }
                    
                    //
                    // calculate P (non-cumulative)
                    //
                    s = 0;
                    for(i=0; i<=npoints-1; i++)
                    {
                        s = s+d2[i];
                    }
                    if( (double)(s)==(double)(0) )
                    {
                        result = false;
                        return result;
                    }
                    s = 1/s;
                    for(i_=0; i_<=npoints-1;i_++)
                    {
                        p[i_] = s*d2[i_];
                    }
                    
                    //
                    // choose one of points with probability P
                    // random number within (0,1) is generated and
                    // inverse empirical CDF is used to randomly choose a point.
                    //
                    s = 0;
                    v = math.randomreal();
                    for(i=0; i<=npoints-1; i++)
                    {
                        s = s+p[i];
                        if( (double)(v)<=(double)(s) | i==npoints-1 )
                        {
                            for(i_=0; i_<=nvars-1;i_++)
                            {
                                centers[cc,i_] = xy[i,i_];
                            }
                            busycenters[cc] = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }


    }
    public class lda
    {
        /*************************************************************************
        Multiclass Fisher LDA

        Subroutine finds coefficients of linear combination which optimally separates
        training set on classes.

        INPUT PARAMETERS:
            XY          -   training set, array[0..NPoints-1,0..NVars].
                            First NVars columns store values of independent
                            variables, next column stores number of class (from 0
                            to NClasses-1) which dataset element belongs to. Fractional
                            values are rounded to nearest integer.
            NPoints     -   training set size, NPoints>=0
            NVars       -   number of independent variables, NVars>=1
            NClasses    -   number of classes, NClasses>=2


        OUTPUT PARAMETERS:
            Info        -   return code:
                            * -4, if internal EVD subroutine hasn't converged
                            * -2, if there is a point with class number
                                  outside of [0..NClasses-1].
                            * -1, if incorrect parameters was passed (NPoints<0,
                                  NVars<1, NClasses<2)
                            *  1, if task has been solved
                            *  2, if there was a multicollinearity in training set,
                                  but task has been solved.
            W           -   linear combination coefficients, array[0..NVars-1]

          -- ALGLIB --
             Copyright 31.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void fisherlda(double[,] xy,
            int npoints,
            int nvars,
            int nclasses,
            ref int info,
            ref double[] w)
        {
            double[,] w2 = new double[0,0];
            int i_ = 0;

            info = 0;
            w = new double[0];

            fisherldan(xy, npoints, nvars, nclasses, ref info, ref w2);
            if( info>0 )
            {
                w = new double[nvars-1+1];
                for(i_=0; i_<=nvars-1;i_++)
                {
                    w[i_] = w2[i_,0];
                }
            }
        }


        /*************************************************************************
        N-dimensional multiclass Fisher LDA

        Subroutine finds coefficients of linear combinations which optimally separates
        training set on classes. It returns N-dimensional basis whose vector are sorted
        by quality of training set separation (in descending order).

        INPUT PARAMETERS:
            XY          -   training set, array[0..NPoints-1,0..NVars].
                            First NVars columns store values of independent
                            variables, next column stores number of class (from 0
                            to NClasses-1) which dataset element belongs to. Fractional
                            values are rounded to nearest integer.
            NPoints     -   training set size, NPoints>=0
            NVars       -   number of independent variables, NVars>=1
            NClasses    -   number of classes, NClasses>=2


        OUTPUT PARAMETERS:
            Info        -   return code:
                            * -4, if internal EVD subroutine hasn't converged
                            * -2, if there is a point with class number
                                  outside of [0..NClasses-1].
                            * -1, if incorrect parameters was passed (NPoints<0,
                                  NVars<1, NClasses<2)
                            *  1, if task has been solved
                            *  2, if there was a multicollinearity in training set,
                                  but task has been solved.
            W           -   basis, array[0..NVars-1,0..NVars-1]
                            columns of matrix stores basis vectors, sorted by
                            quality of training set separation (in descending order)

          -- ALGLIB --
             Copyright 31.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void fisherldan(double[,] xy,
            int npoints,
            int nvars,
            int nclasses,
            ref int info,
            ref double[,] w)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int m = 0;
            double v = 0;
            int[] c = new int[0];
            double[] mu = new double[0];
            double[,] muc = new double[0,0];
            int[] nc = new int[0];
            double[,] sw = new double[0,0];
            double[,] st = new double[0,0];
            double[,] z = new double[0,0];
            double[,] z2 = new double[0,0];
            double[,] tm = new double[0,0];
            double[,] sbroot = new double[0,0];
            double[,] a = new double[0,0];
            double[,] xyproj = new double[0,0];
            double[,] wproj = new double[0,0];
            double[] tf = new double[0];
            double[] d = new double[0];
            double[] d2 = new double[0];
            double[] work = new double[0];
            int i_ = 0;

            info = 0;
            w = new double[0,0];

            
            //
            // Test data
            //
            if( (npoints<0 | nvars<1) | nclasses<2 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=npoints-1; i++)
            {
                if( (int)Math.Round(xy[i,nvars])<0 | (int)Math.Round(xy[i,nvars])>=nclasses )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // Special case: NPoints<=1
            // Degenerate task.
            //
            if( npoints<=1 )
            {
                info = 2;
                w = new double[nvars-1+1, nvars-1+1];
                for(i=0; i<=nvars-1; i++)
                {
                    for(j=0; j<=nvars-1; j++)
                    {
                        if( i==j )
                        {
                            w[i,j] = 1;
                        }
                        else
                        {
                            w[i,j] = 0;
                        }
                    }
                }
                return;
            }
            
            //
            // Prepare temporaries
            //
            tf = new double[nvars-1+1];
            work = new double[Math.Max(nvars, npoints)+1];
            
            //
            // Convert class labels from reals to integers (just for convenience)
            //
            c = new int[npoints-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                c[i] = (int)Math.Round(xy[i,nvars]);
            }
            
            //
            // Calculate class sizes and means
            //
            mu = new double[nvars-1+1];
            muc = new double[nclasses-1+1, nvars-1+1];
            nc = new int[nclasses-1+1];
            for(j=0; j<=nvars-1; j++)
            {
                mu[j] = 0;
            }
            for(i=0; i<=nclasses-1; i++)
            {
                nc[i] = 0;
                for(j=0; j<=nvars-1; j++)
                {
                    muc[i,j] = 0;
                }
            }
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    mu[i_] = mu[i_] + xy[i,i_];
                }
                for(i_=0; i_<=nvars-1;i_++)
                {
                    muc[c[i],i_] = muc[c[i],i_] + xy[i,i_];
                }
                nc[c[i]] = nc[c[i]]+1;
            }
            for(i=0; i<=nclasses-1; i++)
            {
                v = (double)1/(double)nc[i];
                for(i_=0; i_<=nvars-1;i_++)
                {
                    muc[i,i_] = v*muc[i,i_];
                }
            }
            v = (double)1/(double)npoints;
            for(i_=0; i_<=nvars-1;i_++)
            {
                mu[i_] = v*mu[i_];
            }
            
            //
            // Create ST matrix
            //
            st = new double[nvars-1+1, nvars-1+1];
            for(i=0; i<=nvars-1; i++)
            {
                for(j=0; j<=nvars-1; j++)
                {
                    st[i,j] = 0;
                }
            }
            for(k=0; k<=npoints-1; k++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    tf[i_] = xy[k,i_];
                }
                for(i_=0; i_<=nvars-1;i_++)
                {
                    tf[i_] = tf[i_] - mu[i_];
                }
                for(i=0; i<=nvars-1; i++)
                {
                    v = tf[i];
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        st[i,i_] = st[i,i_] + v*tf[i_];
                    }
                }
            }
            
            //
            // Create SW matrix
            //
            sw = new double[nvars-1+1, nvars-1+1];
            for(i=0; i<=nvars-1; i++)
            {
                for(j=0; j<=nvars-1; j++)
                {
                    sw[i,j] = 0;
                }
            }
            for(k=0; k<=npoints-1; k++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    tf[i_] = xy[k,i_];
                }
                for(i_=0; i_<=nvars-1;i_++)
                {
                    tf[i_] = tf[i_] - muc[c[k],i_];
                }
                for(i=0; i<=nvars-1; i++)
                {
                    v = tf[i];
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        sw[i,i_] = sw[i,i_] + v*tf[i_];
                    }
                }
            }
            
            //
            // Maximize ratio J=(w'*ST*w)/(w'*SW*w).
            //
            // First, make transition from w to v such that w'*ST*w becomes v'*v:
            //    v  = root(ST)*w = R*w
            //    R  = root(D)*Z'
            //    w  = (root(ST)^-1)*v = RI*v
            //    RI = Z*inv(root(D))
            //    J  = (v'*v)/(v'*(RI'*SW*RI)*v)
            //    ST = Z*D*Z'
            //
            //    so we have
            //
            //    J = (v'*v) / (v'*(inv(root(D))*Z'*SW*Z*inv(root(D)))*v)  =
            //      = (v'*v) / (v'*A*v)
            //
            if( !evd.smatrixevd(st, nvars, 1, true, ref d, ref z) )
            {
                info = -4;
                return;
            }
            w = new double[nvars-1+1, nvars-1+1];
            if( (double)(d[nvars-1])<=(double)(0) | (double)(d[0])<=(double)(1000*math.machineepsilon*d[nvars-1]) )
            {
                
                //
                // Special case: D[NVars-1]<=0
                // Degenerate task (all variables takes the same value).
                //
                if( (double)(d[nvars-1])<=(double)(0) )
                {
                    info = 2;
                    for(i=0; i<=nvars-1; i++)
                    {
                        for(j=0; j<=nvars-1; j++)
                        {
                            if( i==j )
                            {
                                w[i,j] = 1;
                            }
                            else
                            {
                                w[i,j] = 0;
                            }
                        }
                    }
                    return;
                }
                
                //
                // Special case: degenerate ST matrix, multicollinearity found.
                // Since we know ST eigenvalues/vectors we can translate task to
                // non-degenerate form.
                //
                // Let WG is orthogonal basis of the non zero variance subspace
                // of the ST and let WZ is orthogonal basis of the zero variance
                // subspace.
                //
                // Projection on WG allows us to use LDA on reduced M-dimensional
                // subspace, N-M vectors of WZ allows us to update reduced LDA
                // factors to full N-dimensional subspace.
                //
                m = 0;
                for(k=0; k<=nvars-1; k++)
                {
                    if( (double)(d[k])<=(double)(1000*math.machineepsilon*d[nvars-1]) )
                    {
                        m = k+1;
                    }
                }
                ap.assert(m!=0, "FisherLDAN: internal error #1");
                xyproj = new double[npoints-1+1, nvars-m+1];
                blas.matrixmatrixmultiply(xy, 0, npoints-1, 0, nvars-1, false, z, 0, nvars-1, m, nvars-1, false, 1.0, ref xyproj, 0, npoints-1, 0, nvars-m-1, 0.0, ref work);
                for(i=0; i<=npoints-1; i++)
                {
                    xyproj[i,nvars-m] = xy[i,nvars];
                }
                fisherldan(xyproj, npoints, nvars-m, nclasses, ref info, ref wproj);
                if( info<0 )
                {
                    return;
                }
                blas.matrixmatrixmultiply(z, 0, nvars-1, m, nvars-1, false, wproj, 0, nvars-m-1, 0, nvars-m-1, false, 1.0, ref w, 0, nvars-1, 0, nvars-m-1, 0.0, ref work);
                for(k=nvars-m; k<=nvars-1; k++)
                {
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        w[i_,k] = z[i_,k-(nvars-m)];
                    }
                }
                info = 2;
            }
            else
            {
                
                //
                // General case: no multicollinearity
                //
                tm = new double[nvars-1+1, nvars-1+1];
                a = new double[nvars-1+1, nvars-1+1];
                blas.matrixmatrixmultiply(sw, 0, nvars-1, 0, nvars-1, false, z, 0, nvars-1, 0, nvars-1, false, 1.0, ref tm, 0, nvars-1, 0, nvars-1, 0.0, ref work);
                blas.matrixmatrixmultiply(z, 0, nvars-1, 0, nvars-1, true, tm, 0, nvars-1, 0, nvars-1, false, 1.0, ref a, 0, nvars-1, 0, nvars-1, 0.0, ref work);
                for(i=0; i<=nvars-1; i++)
                {
                    for(j=0; j<=nvars-1; j++)
                    {
                        a[i,j] = a[i,j]/Math.Sqrt(d[i]*d[j]);
                    }
                }
                if( !evd.smatrixevd(a, nvars, 1, true, ref d2, ref z2) )
                {
                    info = -4;
                    return;
                }
                for(k=0; k<=nvars-1; k++)
                {
                    for(i=0; i<=nvars-1; i++)
                    {
                        tf[i] = z2[i,k]/Math.Sqrt(d[i]);
                    }
                    for(i=0; i<=nvars-1; i++)
                    {
                        v = 0.0;
                        for(i_=0; i_<=nvars-1;i_++)
                        {
                            v += z[i,i_]*tf[i_];
                        }
                        w[i,k] = v;
                    }
                }
            }
            
            //
            // Post-processing:
            // * normalization
            // * converting to non-negative form, if possible
            //
            for(k=0; k<=nvars-1; k++)
            {
                v = 0.0;
                for(i_=0; i_<=nvars-1;i_++)
                {
                    v += w[i_,k]*w[i_,k];
                }
                v = 1/Math.Sqrt(v);
                for(i_=0; i_<=nvars-1;i_++)
                {
                    w[i_,k] = v*w[i_,k];
                }
                v = 0;
                for(i=0; i<=nvars-1; i++)
                {
                    v = v+w[i,k];
                }
                if( (double)(v)<(double)(0) )
                {
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        w[i_,k] = -1*w[i_,k];
                    }
                }
            }
        }


    }
    public class linreg
    {
        public class linearmodel
        {
            public double[] w;
            public linearmodel()
            {
                w = new double[0];
            }
        };


        /*************************************************************************
        LRReport structure contains additional information about linear model:
        * C             -   covariation matrix,  array[0..NVars,0..NVars].
                            C[i,j] = Cov(A[i],A[j])
        * RMSError      -   root mean square error on a training set
        * AvgError      -   average error on a training set
        * AvgRelError   -   average relative error on a training set (excluding
                            observations with zero function value).
        * CVRMSError    -   leave-one-out cross-validation estimate of
                            generalization error. Calculated using fast algorithm
                            with O(NVars*NPoints) complexity.
        * CVAvgError    -   cross-validation estimate of average error
        * CVAvgRelError -   cross-validation estimate of average relative error

        All other fields of the structure are intended for internal use and should
        not be used outside ALGLIB.
        *************************************************************************/
        public class lrreport
        {
            public double[,] c;
            public double rmserror;
            public double avgerror;
            public double avgrelerror;
            public double cvrmserror;
            public double cvavgerror;
            public double cvavgrelerror;
            public int ncvdefects;
            public int[] cvdefects;
            public lrreport()
            {
                c = new double[0,0];
                cvdefects = new int[0];
            }
        };




        public const int lrvnum = 5;


        /*************************************************************************
        Linear regression

        Subroutine builds model:

            Y = A(0)*X[0] + ... + A(N-1)*X[N-1] + A(N)

        and model found in ALGLIB format, covariation matrix, training set  errors
        (rms,  average,  average  relative)   and  leave-one-out  cross-validation
        estimate of the generalization error. CV  estimate calculated  using  fast
        algorithm with O(NPoints*NVars) complexity.

        When  covariation  matrix  is  calculated  standard deviations of function
        values are assumed to be equal to RMS error on the training set.

        INPUT PARAMETERS:
            XY          -   training set, array [0..NPoints-1,0..NVars]:
                            * NVars columns - independent variables
                            * last column - dependent variable
            NPoints     -   training set size, NPoints>NVars+1
            NVars       -   number of independent variables

        OUTPUT PARAMETERS:
            Info        -   return code:
                            * -255, in case of unknown internal error
                            * -4, if internal SVD subroutine haven't converged
                            * -1, if incorrect parameters was passed (NPoints<NVars+2, NVars<1).
                            *  1, if subroutine successfully finished
            LM          -   linear model in the ALGLIB format. Use subroutines of
                            this unit to work with the model.
            AR          -   additional results


          -- ALGLIB --
             Copyright 02.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void lrbuild(double[,] xy,
            int npoints,
            int nvars,
            ref int info,
            linearmodel lm,
            lrreport ar)
        {
            double[] s = new double[0];
            int i = 0;
            double sigma2 = 0;
            int i_ = 0;

            info = 0;

            if( npoints<=nvars+1 | nvars<1 )
            {
                info = -1;
                return;
            }
            s = new double[npoints-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                s[i] = 1;
            }
            lrbuilds(xy, s, npoints, nvars, ref info, lm, ar);
            if( info<0 )
            {
                return;
            }
            sigma2 = math.sqr(ar.rmserror)*npoints/(npoints-nvars-1);
            for(i=0; i<=nvars; i++)
            {
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[i,i_] = sigma2*ar.c[i,i_];
                }
            }
        }


        /*************************************************************************
        Linear regression

        Variant of LRBuild which uses vector of standatd deviations (errors in
        function values).

        INPUT PARAMETERS:
            XY          -   training set, array [0..NPoints-1,0..NVars]:
                            * NVars columns - independent variables
                            * last column - dependent variable
            S           -   standard deviations (errors in function values)
                            array[0..NPoints-1], S[i]>0.
            NPoints     -   training set size, NPoints>NVars+1
            NVars       -   number of independent variables

        OUTPUT PARAMETERS:
            Info        -   return code:
                            * -255, in case of unknown internal error
                            * -4, if internal SVD subroutine haven't converged
                            * -1, if incorrect parameters was passed (NPoints<NVars+2, NVars<1).
                            * -2, if S[I]<=0
                            *  1, if subroutine successfully finished
            LM          -   linear model in the ALGLIB format. Use subroutines of
                            this unit to work with the model.
            AR          -   additional results


          -- ALGLIB --
             Copyright 02.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void lrbuilds(double[,] xy,
            double[] s,
            int npoints,
            int nvars,
            ref int info,
            linearmodel lm,
            lrreport ar)
        {
            double[,] xyi = new double[0,0];
            double[] x = new double[0];
            double[] means = new double[0];
            double[] sigmas = new double[0];
            int i = 0;
            int j = 0;
            double v = 0;
            int offs = 0;
            double mean = 0;
            double variance = 0;
            double skewness = 0;
            double kurtosis = 0;
            int i_ = 0;

            info = 0;

            
            //
            // Test parameters
            //
            if( npoints<=nvars+1 | nvars<1 )
            {
                info = -1;
                return;
            }
            
            //
            // Copy data, add one more column (constant term)
            //
            xyi = new double[npoints-1+1, nvars+1+1];
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    xyi[i,i_] = xy[i,i_];
                }
                xyi[i,nvars] = 1;
                xyi[i,nvars+1] = xy[i,nvars];
            }
            
            //
            // Standartization
            //
            x = new double[npoints-1+1];
            means = new double[nvars-1+1];
            sigmas = new double[nvars-1+1];
            for(j=0; j<=nvars-1; j++)
            {
                for(i_=0; i_<=npoints-1;i_++)
                {
                    x[i_] = xy[i_,j];
                }
                basestat.samplemoments(x, npoints, ref mean, ref variance, ref skewness, ref kurtosis);
                means[j] = mean;
                sigmas[j] = Math.Sqrt(variance);
                if( (double)(sigmas[j])==(double)(0) )
                {
                    sigmas[j] = 1;
                }
                for(i=0; i<=npoints-1; i++)
                {
                    xyi[i,j] = (xyi[i,j]-means[j])/sigmas[j];
                }
            }
            
            //
            // Internal processing
            //
            lrinternal(xyi, s, npoints, nvars+1, ref info, lm, ar);
            if( info<0 )
            {
                return;
            }
            
            //
            // Un-standartization
            //
            offs = (int)Math.Round(lm.w[3]);
            for(j=0; j<=nvars-1; j++)
            {
                
                //
                // Constant term is updated (and its covariance too,
                // since it gets some variance from J-th component)
                //
                lm.w[offs+nvars] = lm.w[offs+nvars]-lm.w[offs+j]*means[j]/sigmas[j];
                v = means[j]/sigmas[j];
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[nvars,i_] = ar.c[nvars,i_] - v*ar.c[j,i_];
                }
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[i_,nvars] = ar.c[i_,nvars] - v*ar.c[i_,j];
                }
                
                //
                // J-th term is updated
                //
                lm.w[offs+j] = lm.w[offs+j]/sigmas[j];
                v = 1/sigmas[j];
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[j,i_] = v*ar.c[j,i_];
                }
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[i_,j] = v*ar.c[i_,j];
                }
            }
        }


        /*************************************************************************
        Like LRBuildS, but builds model

            Y = A(0)*X[0] + ... + A(N-1)*X[N-1]

        i.e. with zero constant term.

          -- ALGLIB --
             Copyright 30.10.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void lrbuildzs(double[,] xy,
            double[] s,
            int npoints,
            int nvars,
            ref int info,
            linearmodel lm,
            lrreport ar)
        {
            double[,] xyi = new double[0,0];
            double[] x = new double[0];
            double[] c = new double[0];
            int i = 0;
            int j = 0;
            double v = 0;
            int offs = 0;
            double mean = 0;
            double variance = 0;
            double skewness = 0;
            double kurtosis = 0;
            int i_ = 0;

            info = 0;

            
            //
            // Test parameters
            //
            if( npoints<=nvars+1 | nvars<1 )
            {
                info = -1;
                return;
            }
            
            //
            // Copy data, add one more column (constant term)
            //
            xyi = new double[npoints-1+1, nvars+1+1];
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    xyi[i,i_] = xy[i,i_];
                }
                xyi[i,nvars] = 0;
                xyi[i,nvars+1] = xy[i,nvars];
            }
            
            //
            // Standartization: unusual scaling
            //
            x = new double[npoints-1+1];
            c = new double[nvars-1+1];
            for(j=0; j<=nvars-1; j++)
            {
                for(i_=0; i_<=npoints-1;i_++)
                {
                    x[i_] = xy[i_,j];
                }
                basestat.samplemoments(x, npoints, ref mean, ref variance, ref skewness, ref kurtosis);
                if( (double)(Math.Abs(mean))>(double)(Math.Sqrt(variance)) )
                {
                    
                    //
                    // variation is relatively small, it is better to
                    // bring mean value to 1
                    //
                    c[j] = mean;
                }
                else
                {
                    
                    //
                    // variation is large, it is better to bring variance to 1
                    //
                    if( (double)(variance)==(double)(0) )
                    {
                        variance = 1;
                    }
                    c[j] = Math.Sqrt(variance);
                }
                for(i=0; i<=npoints-1; i++)
                {
                    xyi[i,j] = xyi[i,j]/c[j];
                }
            }
            
            //
            // Internal processing
            //
            lrinternal(xyi, s, npoints, nvars+1, ref info, lm, ar);
            if( info<0 )
            {
                return;
            }
            
            //
            // Un-standartization
            //
            offs = (int)Math.Round(lm.w[3]);
            for(j=0; j<=nvars-1; j++)
            {
                
                //
                // J-th term is updated
                //
                lm.w[offs+j] = lm.w[offs+j]/c[j];
                v = 1/c[j];
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[j,i_] = v*ar.c[j,i_];
                }
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[i_,j] = v*ar.c[i_,j];
                }
            }
        }


        /*************************************************************************
        Like LRBuild but builds model

            Y = A(0)*X[0] + ... + A(N-1)*X[N-1]

        i.e. with zero constant term.

          -- ALGLIB --
             Copyright 30.10.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void lrbuildz(double[,] xy,
            int npoints,
            int nvars,
            ref int info,
            linearmodel lm,
            lrreport ar)
        {
            double[] s = new double[0];
            int i = 0;
            double sigma2 = 0;
            int i_ = 0;

            info = 0;

            if( npoints<=nvars+1 | nvars<1 )
            {
                info = -1;
                return;
            }
            s = new double[npoints-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                s[i] = 1;
            }
            lrbuildzs(xy, s, npoints, nvars, ref info, lm, ar);
            if( info<0 )
            {
                return;
            }
            sigma2 = math.sqr(ar.rmserror)*npoints/(npoints-nvars-1);
            for(i=0; i<=nvars; i++)
            {
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[i,i_] = sigma2*ar.c[i,i_];
                }
            }
        }


        /*************************************************************************
        Unpacks coefficients of linear model.

        INPUT PARAMETERS:
            LM          -   linear model in ALGLIB format

        OUTPUT PARAMETERS:
            V           -   coefficients, array[0..NVars]
                            constant term (intercept) is stored in the V[NVars].
            NVars       -   number of independent variables (one less than number
                            of coefficients)

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void lrunpack(linearmodel lm,
            ref double[] v,
            ref int nvars)
        {
            int offs = 0;
            int i_ = 0;
            int i1_ = 0;

            v = new double[0];
            nvars = 0;

            ap.assert((int)Math.Round(lm.w[1])==lrvnum, "LINREG: Incorrect LINREG version!");
            nvars = (int)Math.Round(lm.w[2]);
            offs = (int)Math.Round(lm.w[3]);
            v = new double[nvars+1];
            i1_ = (offs) - (0);
            for(i_=0; i_<=nvars;i_++)
            {
                v[i_] = lm.w[i_+i1_];
            }
        }


        /*************************************************************************
        "Packs" coefficients and creates linear model in ALGLIB format (LRUnpack
        reversed).

        INPUT PARAMETERS:
            V           -   coefficients, array[0..NVars]
            NVars       -   number of independent variables

        OUTPUT PAREMETERS:
            LM          -   linear model.

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void lrpack(double[] v,
            int nvars,
            linearmodel lm)
        {
            int offs = 0;
            int i_ = 0;
            int i1_ = 0;

            lm.w = new double[4+nvars+1];
            offs = 4;
            lm.w[0] = 4+nvars+1;
            lm.w[1] = lrvnum;
            lm.w[2] = nvars;
            lm.w[3] = offs;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+nvars;i_++)
            {
                lm.w[i_] = v[i_+i1_];
            }
        }


        /*************************************************************************
        Procesing

        INPUT PARAMETERS:
            LM      -   linear model
            X       -   input vector,  array[0..NVars-1].

        Result:
            value of linear model regression estimate

          -- ALGLIB --
             Copyright 03.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double lrprocess(linearmodel lm,
            double[] x)
        {
            double result = 0;
            double v = 0;
            int offs = 0;
            int nvars = 0;
            int i_ = 0;
            int i1_ = 0;

            ap.assert((int)Math.Round(lm.w[1])==lrvnum, "LINREG: Incorrect LINREG version!");
            nvars = (int)Math.Round(lm.w[2]);
            offs = (int)Math.Round(lm.w[3]);
            i1_ = (offs)-(0);
            v = 0.0;
            for(i_=0; i_<=nvars-1;i_++)
            {
                v += x[i_]*lm.w[i_+i1_];
            }
            result = v+lm.w[offs+nvars];
            return result;
        }


        /*************************************************************************
        RMS error on the test set

        INPUT PARAMETERS:
            LM      -   linear model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            root mean square error.

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double lrrmserror(linearmodel lm,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            int i = 0;
            double v = 0;
            int offs = 0;
            int nvars = 0;
            int i_ = 0;
            int i1_ = 0;

            ap.assert((int)Math.Round(lm.w[1])==lrvnum, "LINREG: Incorrect LINREG version!");
            nvars = (int)Math.Round(lm.w[2]);
            offs = (int)Math.Round(lm.w[3]);
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                i1_ = (offs)-(0);
                v = 0.0;
                for(i_=0; i_<=nvars-1;i_++)
                {
                    v += xy[i,i_]*lm.w[i_+i1_];
                }
                v = v+lm.w[offs+nvars];
                result = result+math.sqr(v-xy[i,nvars]);
            }
            result = Math.Sqrt(result/npoints);
            return result;
        }


        /*************************************************************************
        Average error on the test set

        INPUT PARAMETERS:
            LM      -   linear model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            average error.

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double lravgerror(linearmodel lm,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            int i = 0;
            double v = 0;
            int offs = 0;
            int nvars = 0;
            int i_ = 0;
            int i1_ = 0;

            ap.assert((int)Math.Round(lm.w[1])==lrvnum, "LINREG: Incorrect LINREG version!");
            nvars = (int)Math.Round(lm.w[2]);
            offs = (int)Math.Round(lm.w[3]);
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                i1_ = (offs)-(0);
                v = 0.0;
                for(i_=0; i_<=nvars-1;i_++)
                {
                    v += xy[i,i_]*lm.w[i_+i1_];
                }
                v = v+lm.w[offs+nvars];
                result = result+Math.Abs(v-xy[i,nvars]);
            }
            result = result/npoints;
            return result;
        }


        /*************************************************************************
        RMS error on the test set

        INPUT PARAMETERS:
            LM      -   linear model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            average relative error.

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double lravgrelerror(linearmodel lm,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            int i = 0;
            int k = 0;
            double v = 0;
            int offs = 0;
            int nvars = 0;
            int i_ = 0;
            int i1_ = 0;

            ap.assert((int)Math.Round(lm.w[1])==lrvnum, "LINREG: Incorrect LINREG version!");
            nvars = (int)Math.Round(lm.w[2]);
            offs = (int)Math.Round(lm.w[3]);
            result = 0;
            k = 0;
            for(i=0; i<=npoints-1; i++)
            {
                if( (double)(xy[i,nvars])!=(double)(0) )
                {
                    i1_ = (offs)-(0);
                    v = 0.0;
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        v += xy[i,i_]*lm.w[i_+i1_];
                    }
                    v = v+lm.w[offs+nvars];
                    result = result+Math.Abs((v-xy[i,nvars])/xy[i,nvars]);
                    k = k+1;
                }
            }
            if( k!=0 )
            {
                result = result/k;
            }
            return result;
        }


        /*************************************************************************
        Copying of LinearModel strucure

        INPUT PARAMETERS:
            LM1 -   original

        OUTPUT PARAMETERS:
            LM2 -   copy

          -- ALGLIB --
             Copyright 15.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void lrcopy(linearmodel lm1,
            linearmodel lm2)
        {
            int k = 0;
            int i_ = 0;

            k = (int)Math.Round(lm1.w[0]);
            lm2.w = new double[k-1+1];
            for(i_=0; i_<=k-1;i_++)
            {
                lm2.w[i_] = lm1.w[i_];
            }
        }


        public static void lrlines(double[,] xy,
            double[] s,
            int n,
            ref int info,
            ref double a,
            ref double b,
            ref double vara,
            ref double varb,
            ref double covab,
            ref double corrab,
            ref double p)
        {
            int i = 0;
            double ss = 0;
            double sx = 0;
            double sxx = 0;
            double sy = 0;
            double stt = 0;
            double e1 = 0;
            double e2 = 0;
            double t = 0;
            double chi2 = 0;

            info = 0;
            a = 0;
            b = 0;
            vara = 0;
            varb = 0;
            covab = 0;
            corrab = 0;
            p = 0;

            if( n<2 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( (double)(s[i])<=(double)(0) )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // Calculate S, SX, SY, SXX
            //
            ss = 0;
            sx = 0;
            sy = 0;
            sxx = 0;
            for(i=0; i<=n-1; i++)
            {
                t = math.sqr(s[i]);
                ss = ss+1/t;
                sx = sx+xy[i,0]/t;
                sy = sy+xy[i,1]/t;
                sxx = sxx+math.sqr(xy[i,0])/t;
            }
            
            //
            // Test for condition number
            //
            t = Math.Sqrt(4*math.sqr(sx)+math.sqr(ss-sxx));
            e1 = 0.5*(ss+sxx+t);
            e2 = 0.5*(ss+sxx-t);
            if( (double)(Math.Min(e1, e2))<=(double)(1000*math.machineepsilon*Math.Max(e1, e2)) )
            {
                info = -3;
                return;
            }
            
            //
            // Calculate A, B
            //
            a = 0;
            b = 0;
            stt = 0;
            for(i=0; i<=n-1; i++)
            {
                t = (xy[i,0]-sx/ss)/s[i];
                b = b+t*xy[i,1]/s[i];
                stt = stt+math.sqr(t);
            }
            b = b/stt;
            a = (sy-sx*b)/ss;
            
            //
            // Calculate goodness-of-fit
            //
            if( n>2 )
            {
                chi2 = 0;
                for(i=0; i<=n-1; i++)
                {
                    chi2 = chi2+math.sqr((xy[i,1]-a-b*xy[i,0])/s[i]);
                }
                p = igammaf.incompletegammac((double)(n-2)/(double)2, chi2/2);
            }
            else
            {
                p = 1;
            }
            
            //
            // Calculate other parameters
            //
            vara = (1+math.sqr(sx)/(ss*stt))/ss;
            varb = 1/stt;
            covab = -(sx/(ss*stt));
            corrab = covab/Math.Sqrt(vara*varb);
        }


        public static void lrline(double[,] xy,
            int n,
            ref int info,
            ref double a,
            ref double b)
        {
            double[] s = new double[0];
            int i = 0;
            double vara = 0;
            double varb = 0;
            double covab = 0;
            double corrab = 0;
            double p = 0;

            info = 0;
            a = 0;
            b = 0;

            if( n<2 )
            {
                info = -1;
                return;
            }
            s = new double[n-1+1];
            for(i=0; i<=n-1; i++)
            {
                s[i] = 1;
            }
            lrlines(xy, s, n, ref info, ref a, ref b, ref vara, ref varb, ref covab, ref corrab, ref p);
        }


        /*************************************************************************
        Internal linear regression subroutine
        *************************************************************************/
        private static void lrinternal(double[,] xy,
            double[] s,
            int npoints,
            int nvars,
            ref int info,
            linearmodel lm,
            lrreport ar)
        {
            double[,] a = new double[0,0];
            double[,] u = new double[0,0];
            double[,] vt = new double[0,0];
            double[,] vm = new double[0,0];
            double[,] xym = new double[0,0];
            double[] b = new double[0];
            double[] sv = new double[0];
            double[] t = new double[0];
            double[] svi = new double[0];
            double[] work = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            int ncv = 0;
            int na = 0;
            int nacv = 0;
            double r = 0;
            double p = 0;
            double epstol = 0;
            lrreport ar2 = new lrreport();
            int offs = 0;
            linearmodel tlm = new linearmodel();
            int i_ = 0;
            int i1_ = 0;

            info = 0;

            epstol = 1000;
            
            //
            // Check for errors in data
            //
            if( npoints<nvars | nvars<1 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=npoints-1; i++)
            {
                if( (double)(s[i])<=(double)(0) )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // Create design matrix
            //
            a = new double[npoints-1+1, nvars-1+1];
            b = new double[npoints-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                r = 1/s[i];
                for(i_=0; i_<=nvars-1;i_++)
                {
                    a[i,i_] = r*xy[i,i_];
                }
                b[i] = xy[i,nvars]/s[i];
            }
            
            //
            // Allocate W:
            // W[0]     array size
            // W[1]     version number, 0
            // W[2]     NVars (minus 1, to be compatible with external representation)
            // W[3]     coefficients offset
            //
            lm.w = new double[4+nvars-1+1];
            offs = 4;
            lm.w[0] = 4+nvars;
            lm.w[1] = lrvnum;
            lm.w[2] = nvars-1;
            lm.w[3] = offs;
            
            //
            // Solve problem using SVD:
            //
            // 0. check for degeneracy (different types)
            // 1. A = U*diag(sv)*V'
            // 2. T = b'*U
            // 3. w = SUM((T[i]/sv[i])*V[..,i])
            // 4. cov(wi,wj) = SUM(Vji*Vjk/sv[i]^2,K=1..M)
            //
            // see $15.4 of "Numerical Recipes in C" for more information
            //
            t = new double[nvars-1+1];
            svi = new double[nvars-1+1];
            ar.c = new double[nvars-1+1, nvars-1+1];
            vm = new double[nvars-1+1, nvars-1+1];
            if( !svd.rmatrixsvd(a, npoints, nvars, 1, 1, 2, ref sv, ref u, ref vt) )
            {
                info = -4;
                return;
            }
            if( (double)(sv[0])<=(double)(0) )
            {
                
                //
                // Degenerate case: zero design matrix.
                //
                for(i=offs; i<=offs+nvars-1; i++)
                {
                    lm.w[i] = 0;
                }
                ar.rmserror = lrrmserror(lm, xy, npoints);
                ar.avgerror = lravgerror(lm, xy, npoints);
                ar.avgrelerror = lravgrelerror(lm, xy, npoints);
                ar.cvrmserror = ar.rmserror;
                ar.cvavgerror = ar.avgerror;
                ar.cvavgrelerror = ar.avgrelerror;
                ar.ncvdefects = 0;
                ar.cvdefects = new int[nvars-1+1];
                ar.c = new double[nvars-1+1, nvars-1+1];
                for(i=0; i<=nvars-1; i++)
                {
                    for(j=0; j<=nvars-1; j++)
                    {
                        ar.c[i,j] = 0;
                    }
                }
                return;
            }
            if( (double)(sv[nvars-1])<=(double)(epstol*math.machineepsilon*sv[0]) )
            {
                
                //
                // Degenerate case, non-zero design matrix.
                //
                // We can leave it and solve task in SVD least squares fashion.
                // Solution and covariance matrix will be obtained correctly,
                // but CV error estimates - will not. It is better to reduce
                // it to non-degenerate task and to obtain correct CV estimates.
                //
                for(k=nvars; k>=1; k--)
                {
                    if( (double)(sv[k-1])>(double)(epstol*math.machineepsilon*sv[0]) )
                    {
                        
                        //
                        // Reduce
                        //
                        xym = new double[npoints-1+1, k+1];
                        for(i=0; i<=npoints-1; i++)
                        {
                            for(j=0; j<=k-1; j++)
                            {
                                r = 0.0;
                                for(i_=0; i_<=nvars-1;i_++)
                                {
                                    r += xy[i,i_]*vt[j,i_];
                                }
                                xym[i,j] = r;
                            }
                            xym[i,k] = xy[i,nvars];
                        }
                        
                        //
                        // Solve
                        //
                        lrinternal(xym, s, npoints, k, ref info, tlm, ar2);
                        if( info!=1 )
                        {
                            return;
                        }
                        
                        //
                        // Convert back to un-reduced format
                        //
                        for(j=0; j<=nvars-1; j++)
                        {
                            lm.w[offs+j] = 0;
                        }
                        for(j=0; j<=k-1; j++)
                        {
                            r = tlm.w[offs+j];
                            i1_ = (0) - (offs);
                            for(i_=offs; i_<=offs+nvars-1;i_++)
                            {
                                lm.w[i_] = lm.w[i_] + r*vt[j,i_+i1_];
                            }
                        }
                        ar.rmserror = ar2.rmserror;
                        ar.avgerror = ar2.avgerror;
                        ar.avgrelerror = ar2.avgrelerror;
                        ar.cvrmserror = ar2.cvrmserror;
                        ar.cvavgerror = ar2.cvavgerror;
                        ar.cvavgrelerror = ar2.cvavgrelerror;
                        ar.ncvdefects = ar2.ncvdefects;
                        ar.cvdefects = new int[nvars-1+1];
                        for(j=0; j<=ar.ncvdefects-1; j++)
                        {
                            ar.cvdefects[j] = ar2.cvdefects[j];
                        }
                        ar.c = new double[nvars-1+1, nvars-1+1];
                        work = new double[nvars+1];
                        blas.matrixmatrixmultiply(ar2.c, 0, k-1, 0, k-1, false, vt, 0, k-1, 0, nvars-1, false, 1.0, ref vm, 0, k-1, 0, nvars-1, 0.0, ref work);
                        blas.matrixmatrixmultiply(vt, 0, k-1, 0, nvars-1, true, vm, 0, k-1, 0, nvars-1, false, 1.0, ref ar.c, 0, nvars-1, 0, nvars-1, 0.0, ref work);
                        return;
                    }
                }
                info = -255;
                return;
            }
            for(i=0; i<=nvars-1; i++)
            {
                if( (double)(sv[i])>(double)(epstol*math.machineepsilon*sv[0]) )
                {
                    svi[i] = 1/sv[i];
                }
                else
                {
                    svi[i] = 0;
                }
            }
            for(i=0; i<=nvars-1; i++)
            {
                t[i] = 0;
            }
            for(i=0; i<=npoints-1; i++)
            {
                r = b[i];
                for(i_=0; i_<=nvars-1;i_++)
                {
                    t[i_] = t[i_] + r*u[i,i_];
                }
            }
            for(i=0; i<=nvars-1; i++)
            {
                lm.w[offs+i] = 0;
            }
            for(i=0; i<=nvars-1; i++)
            {
                r = t[i]*svi[i];
                i1_ = (0) - (offs);
                for(i_=offs; i_<=offs+nvars-1;i_++)
                {
                    lm.w[i_] = lm.w[i_] + r*vt[i,i_+i1_];
                }
            }
            for(j=0; j<=nvars-1; j++)
            {
                r = svi[j];
                for(i_=0; i_<=nvars-1;i_++)
                {
                    vm[i_,j] = r*vt[j,i_];
                }
            }
            for(i=0; i<=nvars-1; i++)
            {
                for(j=i; j<=nvars-1; j++)
                {
                    r = 0.0;
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        r += vm[i,i_]*vm[j,i_];
                    }
                    ar.c[i,j] = r;
                    ar.c[j,i] = r;
                }
            }
            
            //
            // Leave-1-out cross-validation error.
            //
            // NOTATIONS:
            // A            design matrix
            // A*x = b      original linear least squares task
            // U*S*V'       SVD of A
            // ai           i-th row of the A
            // bi           i-th element of the b
            // xf           solution of the original LLS task
            //
            // Cross-validation error of i-th element from a sample is
            // calculated using following formula:
            //
            //     ERRi = ai*xf - (ai*xf-bi*(ui*ui'))/(1-ui*ui')     (1)
            //
            // This formula can be derived from normal equations of the
            // original task
            //
            //     (A'*A)x = A'*b                                    (2)
            //
            // by applying modification (zeroing out i-th row of A) to (2):
            //
            //     (A-ai)'*(A-ai) = (A-ai)'*b
            //
            // and using Sherman-Morrison formula for updating matrix inverse
            //
            // NOTE 1: b is not zeroed out since it is much simpler and
            // does not influence final result.
            //
            // NOTE 2: some design matrices A have such ui that 1-ui*ui'=0.
            // Formula (1) can't be applied for such cases and they are skipped
            // from CV calculation (which distorts resulting CV estimate).
            // But from the properties of U we can conclude that there can
            // be no more than NVars such vectors. Usually
            // NVars << NPoints, so in a normal case it only slightly
            // influences result.
            //
            ncv = 0;
            na = 0;
            nacv = 0;
            ar.rmserror = 0;
            ar.avgerror = 0;
            ar.avgrelerror = 0;
            ar.cvrmserror = 0;
            ar.cvavgerror = 0;
            ar.cvavgrelerror = 0;
            ar.ncvdefects = 0;
            ar.cvdefects = new int[nvars-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                
                //
                // Error on a training set
                //
                i1_ = (offs)-(0);
                r = 0.0;
                for(i_=0; i_<=nvars-1;i_++)
                {
                    r += xy[i,i_]*lm.w[i_+i1_];
                }
                ar.rmserror = ar.rmserror+math.sqr(r-xy[i,nvars]);
                ar.avgerror = ar.avgerror+Math.Abs(r-xy[i,nvars]);
                if( (double)(xy[i,nvars])!=(double)(0) )
                {
                    ar.avgrelerror = ar.avgrelerror+Math.Abs((r-xy[i,nvars])/xy[i,nvars]);
                    na = na+1;
                }
                
                //
                // Error using fast leave-one-out cross-validation
                //
                p = 0.0;
                for(i_=0; i_<=nvars-1;i_++)
                {
                    p += u[i,i_]*u[i,i_];
                }
                if( (double)(p)>(double)(1-epstol*math.machineepsilon) )
                {
                    ar.cvdefects[ar.ncvdefects] = i;
                    ar.ncvdefects = ar.ncvdefects+1;
                    continue;
                }
                r = s[i]*(r/s[i]-b[i]*p)/(1-p);
                ar.cvrmserror = ar.cvrmserror+math.sqr(r-xy[i,nvars]);
                ar.cvavgerror = ar.cvavgerror+Math.Abs(r-xy[i,nvars]);
                if( (double)(xy[i,nvars])!=(double)(0) )
                {
                    ar.cvavgrelerror = ar.cvavgrelerror+Math.Abs((r-xy[i,nvars])/xy[i,nvars]);
                    nacv = nacv+1;
                }
                ncv = ncv+1;
            }
            if( ncv==0 )
            {
                
                //
                // Something strange: ALL ui are degenerate.
                // Unexpected...
                //
                info = -255;
                return;
            }
            ar.rmserror = Math.Sqrt(ar.rmserror/npoints);
            ar.avgerror = ar.avgerror/npoints;
            if( na!=0 )
            {
                ar.avgrelerror = ar.avgrelerror/na;
            }
            ar.cvrmserror = Math.Sqrt(ar.cvrmserror/ncv);
            ar.cvavgerror = ar.cvavgerror/ncv;
            if( nacv!=0 )
            {
                ar.cvavgrelerror = ar.cvavgrelerror/nacv;
            }
        }


    }
    public class mlpbase
    {
        public class multilayerperceptron
        {
            public int[] structinfo;
            public double[] weights;
            public double[] columnmeans;
            public double[] columnsigmas;
            public double[] neurons;
            public double[] dfdnet;
            public double[] derror;
            public double[] x;
            public double[] y;
            public double[,] chunks;
            public double[] nwbuf;
            public multilayerperceptron()
            {
                structinfo = new int[0];
                weights = new double[0];
                columnmeans = new double[0];
                columnsigmas = new double[0];
                neurons = new double[0];
                dfdnet = new double[0];
                derror = new double[0];
                x = new double[0];
                y = new double[0];
                chunks = new double[0,0];
                nwbuf = new double[0];
            }
        };




        public const int mlpvnum = 7;
        public const int nfieldwidth = 4;
        public const int chunksize = 32;


        /*************************************************************************
        Creates  neural  network  with  NIn  inputs,  NOut outputs, without hidden
        layers, with linear output layer. Network weights are  filled  with  small
        random values.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreate0(int nin,
            int nout,
            multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;

            layerscount = 1+2;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, lsizes, ltypes, lconnfirst, lconnlast, layerscount, false, network);
        }


        /*************************************************************************
        Same  as  MLPCreate0,  but  with  one  hidden  layer  (NHid  neurons) with
        non-linear activation function. Output layer is linear.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreate1(int nin,
            int nhid,
            int nout,
            multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;

            layerscount = 1+3+2;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, lsizes, ltypes, lconnfirst, lconnlast, layerscount, false, network);
        }


        /*************************************************************************
        Same as MLPCreate0, but with two hidden layers (NHid1 and  NHid2  neurons)
        with non-linear activation function. Output layer is linear.
         $ALL

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreate2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;

            layerscount = 1+3+3+2;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid2, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, lsizes, ltypes, lconnfirst, lconnlast, layerscount, false, network);
        }


        /*************************************************************************
        Creates  neural  network  with  NIn  inputs,  NOut outputs, without hidden
        layers with non-linear output layer. Network weights are filled with small
        random values.

        Activation function of the output layer takes values:

            (B, +INF), if D>=0

        or

            (-INF, B), if D<0.


          -- ALGLIB --
             Copyright 30.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreateb0(int nin,
            int nout,
            double b,
            double d,
            multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;
            int i = 0;

            layerscount = 1+3;
            if( (double)(d)>=(double)(0) )
            {
                d = 1;
            }
            else
            {
                d = -1;
            }
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(3, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, lsizes, ltypes, lconnfirst, lconnlast, layerscount, false, network);
            
            //
            // Turn on ouputs shift/scaling.
            //
            for(i=nin; i<=nin+nout-1; i++)
            {
                network.columnmeans[i] = b;
                network.columnsigmas[i] = d;
            }
        }


        /*************************************************************************
        Same as MLPCreateB0 but with non-linear hidden layer.

          -- ALGLIB --
             Copyright 30.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreateb1(int nin,
            int nhid,
            int nout,
            double b,
            double d,
            multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;
            int i = 0;

            layerscount = 1+3+3;
            if( (double)(d)>=(double)(0) )
            {
                d = 1;
            }
            else
            {
                d = -1;
            }
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(3, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, lsizes, ltypes, lconnfirst, lconnlast, layerscount, false, network);
            
            //
            // Turn on ouputs shift/scaling.
            //
            for(i=nin; i<=nin+nout-1; i++)
            {
                network.columnmeans[i] = b;
                network.columnsigmas[i] = d;
            }
        }


        /*************************************************************************
        Same as MLPCreateB0 but with two non-linear hidden layers.

          -- ALGLIB --
             Copyright 30.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreateb2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            double b,
            double d,
            multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;
            int i = 0;

            layerscount = 1+3+3+3;
            if( (double)(d)>=(double)(0) )
            {
                d = 1;
            }
            else
            {
                d = -1;
            }
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid2, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(3, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, lsizes, ltypes, lconnfirst, lconnlast, layerscount, false, network);
            
            //
            // Turn on ouputs shift/scaling.
            //
            for(i=nin; i<=nin+nout-1; i++)
            {
                network.columnmeans[i] = b;
                network.columnsigmas[i] = d;
            }
        }


        /*************************************************************************
        Creates  neural  network  with  NIn  inputs,  NOut outputs, without hidden
        layers with non-linear output layer. Network weights are filled with small
        random values. Activation function of the output layer takes values [A,B].

          -- ALGLIB --
             Copyright 30.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreater0(int nin,
            int nout,
            double a,
            double b,
            multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;
            int i = 0;

            layerscount = 1+3;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, lsizes, ltypes, lconnfirst, lconnlast, layerscount, false, network);
            
            //
            // Turn on outputs shift/scaling.
            //
            for(i=nin; i<=nin+nout-1; i++)
            {
                network.columnmeans[i] = 0.5*(a+b);
                network.columnsigmas[i] = 0.5*(a-b);
            }
        }


        /*************************************************************************
        Same as MLPCreateR0, but with non-linear hidden layer.

          -- ALGLIB --
             Copyright 30.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreater1(int nin,
            int nhid,
            int nout,
            double a,
            double b,
            multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;
            int i = 0;

            layerscount = 1+3+3;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, lsizes, ltypes, lconnfirst, lconnlast, layerscount, false, network);
            
            //
            // Turn on outputs shift/scaling.
            //
            for(i=nin; i<=nin+nout-1; i++)
            {
                network.columnmeans[i] = 0.5*(a+b);
                network.columnsigmas[i] = 0.5*(a-b);
            }
        }


        /*************************************************************************
        Same as MLPCreateR0, but with two non-linear hidden layers.

          -- ALGLIB --
             Copyright 30.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreater2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            double a,
            double b,
            multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;
            int i = 0;

            layerscount = 1+3+3+3;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid2, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, lsizes, ltypes, lconnfirst, lconnlast, layerscount, false, network);
            
            //
            // Turn on outputs shift/scaling.
            //
            for(i=nin; i<=nin+nout-1; i++)
            {
                network.columnmeans[i] = 0.5*(a+b);
                network.columnsigmas[i] = 0.5*(a-b);
            }
        }


        /*************************************************************************
        Creates classifier network with NIn  inputs  and  NOut  possible  classes.
        Network contains no hidden layers and linear output  layer  with  SOFTMAX-
        normalization  (so  outputs  sums  up  to  1.0  and  converge to posterior
        probabilities).

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreatec0(int nin,
            int nout,
            multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;

            ap.assert(nout>=2, "MLPCreateC0: NOut<2!");
            layerscount = 1+2+1;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout-1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addzerolayer(ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, lsizes, ltypes, lconnfirst, lconnlast, layerscount, true, network);
        }


        /*************************************************************************
        Same as MLPCreateC0, but with one non-linear hidden layer.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreatec1(int nin,
            int nhid,
            int nout,
            multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;

            ap.assert(nout>=2, "MLPCreateC1: NOut<2!");
            layerscount = 1+3+2+1;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout-1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addzerolayer(ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, lsizes, ltypes, lconnfirst, lconnlast, layerscount, true, network);
        }


        /*************************************************************************
        Same as MLPCreateC0, but with two non-linear hidden layers.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreatec2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;

            ap.assert(nout>=2, "MLPCreateC2: NOut<2!");
            layerscount = 1+3+3+2+1;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid2, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout-1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addzerolayer(ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, lsizes, ltypes, lconnfirst, lconnlast, layerscount, true, network);
        }


        /*************************************************************************
        Copying of neural network

        INPUT PARAMETERS:
            Network1 -   original

        OUTPUT PARAMETERS:
            Network2 -   copy

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcopy(multilayerperceptron network1,
            multilayerperceptron network2)
        {
            int i = 0;
            int ssize = 0;
            int ntotal = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int i_ = 0;

            
            //
            // Unload info
            //
            ssize = network1.structinfo[0];
            nin = network1.structinfo[1];
            nout = network1.structinfo[2];
            ntotal = network1.structinfo[3];
            wcount = network1.structinfo[4];
            
            //
            // Allocate space
            //
            network2.structinfo = new int[ssize-1+1];
            network2.weights = new double[wcount-1+1];
            if( mlpissoftmax(network1) )
            {
                network2.columnmeans = new double[nin-1+1];
                network2.columnsigmas = new double[nin-1+1];
            }
            else
            {
                network2.columnmeans = new double[nin+nout-1+1];
                network2.columnsigmas = new double[nin+nout-1+1];
            }
            network2.neurons = new double[ntotal-1+1];
            network2.chunks = new double[3*ntotal+1, chunksize-1+1];
            network2.nwbuf = new double[Math.Max(wcount, 2*nout)-1+1];
            network2.dfdnet = new double[ntotal-1+1];
            network2.x = new double[nin-1+1];
            network2.y = new double[nout-1+1];
            network2.derror = new double[ntotal-1+1];
            
            //
            // Copy
            //
            for(i=0; i<=ssize-1; i++)
            {
                network2.structinfo[i] = network1.structinfo[i];
            }
            for(i_=0; i_<=wcount-1;i_++)
            {
                network2.weights[i_] = network1.weights[i_];
            }
            if( mlpissoftmax(network1) )
            {
                for(i_=0; i_<=nin-1;i_++)
                {
                    network2.columnmeans[i_] = network1.columnmeans[i_];
                }
                for(i_=0; i_<=nin-1;i_++)
                {
                    network2.columnsigmas[i_] = network1.columnsigmas[i_];
                }
            }
            else
            {
                for(i_=0; i_<=nin+nout-1;i_++)
                {
                    network2.columnmeans[i_] = network1.columnmeans[i_];
                }
                for(i_=0; i_<=nin+nout-1;i_++)
                {
                    network2.columnsigmas[i_] = network1.columnsigmas[i_];
                }
            }
            for(i_=0; i_<=ntotal-1;i_++)
            {
                network2.neurons[i_] = network1.neurons[i_];
            }
            for(i_=0; i_<=ntotal-1;i_++)
            {
                network2.dfdnet[i_] = network1.dfdnet[i_];
            }
            for(i_=0; i_<=nin-1;i_++)
            {
                network2.x[i_] = network1.x[i_];
            }
            for(i_=0; i_<=nout-1;i_++)
            {
                network2.y[i_] = network1.y[i_];
            }
            for(i_=0; i_<=ntotal-1;i_++)
            {
                network2.derror[i_] = network1.derror[i_];
            }
        }


        /*************************************************************************
        Serialization of MultiLayerPerceptron strucure

        INPUT PARAMETERS:
            Network -   original

        OUTPUT PARAMETERS:
            RA      -   array of real numbers which stores network,
                        array[0..RLen-1]
            RLen    -   RA lenght

          -- ALGLIB --
             Copyright 29.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpserialize(multilayerperceptron network,
            ref double[] ra,
            ref int rlen)
        {
            int i = 0;
            int ssize = 0;
            int ntotal = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int sigmalen = 0;
            int offs = 0;
            int i_ = 0;
            int i1_ = 0;

            ra = new double[0];
            rlen = 0;

            
            //
            // Unload info
            //
            ssize = network.structinfo[0];
            nin = network.structinfo[1];
            nout = network.structinfo[2];
            ntotal = network.structinfo[3];
            wcount = network.structinfo[4];
            if( mlpissoftmax(network) )
            {
                sigmalen = nin;
            }
            else
            {
                sigmalen = nin+nout;
            }
            
            //
            //  RA format:
            //      LEN         DESRC.
            //      1           RLen
            //      1           version (MLPVNum)
            //      1           StructInfo size
            //      SSize       StructInfo
            //      WCount      Weights
            //      SigmaLen    ColumnMeans
            //      SigmaLen    ColumnSigmas
            //
            rlen = 3+ssize+wcount+2*sigmalen;
            ra = new double[rlen-1+1];
            ra[0] = rlen;
            ra[1] = mlpvnum;
            ra[2] = ssize;
            offs = 3;
            for(i=0; i<=ssize-1; i++)
            {
                ra[offs+i] = network.structinfo[i];
            }
            offs = offs+ssize;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+wcount-1;i_++)
            {
                ra[i_] = network.weights[i_+i1_];
            }
            offs = offs+wcount;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+sigmalen-1;i_++)
            {
                ra[i_] = network.columnmeans[i_+i1_];
            }
            offs = offs+sigmalen;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+sigmalen-1;i_++)
            {
                ra[i_] = network.columnsigmas[i_+i1_];
            }
            offs = offs+sigmalen;
        }


        /*************************************************************************
        Unserialization of MultiLayerPerceptron strucure

        INPUT PARAMETERS:
            RA      -   real array which stores network

        OUTPUT PARAMETERS:
            Network -   restored network

          -- ALGLIB --
             Copyright 29.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpunserialize(double[] ra,
            multilayerperceptron network)
        {
            int i = 0;
            int ssize = 0;
            int ntotal = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int sigmalen = 0;
            int offs = 0;
            int i_ = 0;
            int i1_ = 0;

            ap.assert((int)Math.Round(ra[1])==mlpvnum, "MLPUnserialize: incorrect array!");
            
            //
            // Unload StructInfo from IA
            //
            offs = 3;
            ssize = (int)Math.Round(ra[2]);
            network.structinfo = new int[ssize-1+1];
            for(i=0; i<=ssize-1; i++)
            {
                network.structinfo[i] = (int)Math.Round(ra[offs+i]);
            }
            offs = offs+ssize;
            
            //
            // Unload info from StructInfo
            //
            ssize = network.structinfo[0];
            nin = network.structinfo[1];
            nout = network.structinfo[2];
            ntotal = network.structinfo[3];
            wcount = network.structinfo[4];
            if( network.structinfo[6]==0 )
            {
                sigmalen = nin+nout;
            }
            else
            {
                sigmalen = nin;
            }
            
            //
            // Allocate space for other fields
            //
            network.weights = new double[wcount-1+1];
            network.columnmeans = new double[sigmalen-1+1];
            network.columnsigmas = new double[sigmalen-1+1];
            network.neurons = new double[ntotal-1+1];
            network.chunks = new double[3*ntotal+1, chunksize-1+1];
            network.nwbuf = new double[Math.Max(wcount, 2*nout)-1+1];
            network.dfdnet = new double[ntotal-1+1];
            network.x = new double[nin-1+1];
            network.y = new double[nout-1+1];
            network.derror = new double[ntotal-1+1];
            
            //
            // Copy parameters from RA
            //
            i1_ = (offs) - (0);
            for(i_=0; i_<=wcount-1;i_++)
            {
                network.weights[i_] = ra[i_+i1_];
            }
            offs = offs+wcount;
            i1_ = (offs) - (0);
            for(i_=0; i_<=sigmalen-1;i_++)
            {
                network.columnmeans[i_] = ra[i_+i1_];
            }
            offs = offs+sigmalen;
            i1_ = (offs) - (0);
            for(i_=0; i_<=sigmalen-1;i_++)
            {
                network.columnsigmas[i_] = ra[i_+i1_];
            }
            offs = offs+sigmalen;
        }


        /*************************************************************************
        Randomization of neural network weights

          -- ALGLIB --
             Copyright 06.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlprandomize(multilayerperceptron network)
        {
            int i = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;

            mlpproperties(network, ref nin, ref nout, ref wcount);
            for(i=0; i<=wcount-1; i++)
            {
                network.weights[i] = math.randomreal()-0.5;
            }
        }


        /*************************************************************************
        Randomization of neural network weights and standartisator

          -- ALGLIB --
             Copyright 10.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlprandomizefull(multilayerperceptron network)
        {
            int i = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int ntotal = 0;
            int istart = 0;
            int offs = 0;
            int ntype = 0;

            mlpproperties(network, ref nin, ref nout, ref wcount);
            ntotal = network.structinfo[3];
            istart = network.structinfo[5];
            
            //
            // Process network
            //
            for(i=0; i<=wcount-1; i++)
            {
                network.weights[i] = math.randomreal()-0.5;
            }
            for(i=0; i<=nin-1; i++)
            {
                network.columnmeans[i] = 2*math.randomreal()-1;
                network.columnsigmas[i] = 1.5*math.randomreal()+0.5;
            }
            if( !mlpissoftmax(network) )
            {
                for(i=0; i<=nout-1; i++)
                {
                    offs = istart+(ntotal-nout+i)*nfieldwidth;
                    ntype = network.structinfo[offs+0];
                    if( ntype==0 )
                    {
                        
                        //
                        // Shifts are changed only for linear outputs neurons
                        //
                        network.columnmeans[nin+i] = 2*math.randomreal()-1;
                    }
                    if( ntype==0 | ntype==3 )
                    {
                        
                        //
                        // Scales are changed only for linear or bounded outputs neurons.
                        // Note that scale randomization preserves sign.
                        //
                        network.columnsigmas[nin+i] = Math.Sign(network.columnsigmas[nin+i])*(1.5*math.randomreal()+0.5);
                    }
                }
            }
        }


        /*************************************************************************
        Internal subroutine.

          -- ALGLIB --
             Copyright 30.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpinitpreprocessor(multilayerperceptron network,
            double[,] xy,
            int ssize)
        {
            int i = 0;
            int j = 0;
            int jmax = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int ntotal = 0;
            int istart = 0;
            int offs = 0;
            int ntype = 0;
            double[] means = new double[0];
            double[] sigmas = new double[0];
            double s = 0;

            mlpproperties(network, ref nin, ref nout, ref wcount);
            ntotal = network.structinfo[3];
            istart = network.structinfo[5];
            
            //
            // Means/Sigmas
            //
            if( mlpissoftmax(network) )
            {
                jmax = nin-1;
            }
            else
            {
                jmax = nin+nout-1;
            }
            means = new double[jmax+1];
            sigmas = new double[jmax+1];
            for(j=0; j<=jmax; j++)
            {
                means[j] = 0;
                for(i=0; i<=ssize-1; i++)
                {
                    means[j] = means[j]+xy[i,j];
                }
                means[j] = means[j]/ssize;
                sigmas[j] = 0;
                for(i=0; i<=ssize-1; i++)
                {
                    sigmas[j] = sigmas[j]+math.sqr(xy[i,j]-means[j]);
                }
                sigmas[j] = Math.Sqrt(sigmas[j]/ssize);
            }
            
            //
            // Inputs
            //
            for(i=0; i<=nin-1; i++)
            {
                network.columnmeans[i] = means[i];
                network.columnsigmas[i] = sigmas[i];
                if( (double)(network.columnsigmas[i])==(double)(0) )
                {
                    network.columnsigmas[i] = 1;
                }
            }
            
            //
            // Outputs
            //
            if( !mlpissoftmax(network) )
            {
                for(i=0; i<=nout-1; i++)
                {
                    offs = istart+(ntotal-nout+i)*nfieldwidth;
                    ntype = network.structinfo[offs+0];
                    
                    //
                    // Linear outputs
                    //
                    if( ntype==0 )
                    {
                        network.columnmeans[nin+i] = means[nin+i];
                        network.columnsigmas[nin+i] = sigmas[nin+i];
                        if( (double)(network.columnsigmas[nin+i])==(double)(0) )
                        {
                            network.columnsigmas[nin+i] = 1;
                        }
                    }
                    
                    //
                    // Bounded outputs (half-interval)
                    //
                    if( ntype==3 )
                    {
                        s = means[nin+i]-network.columnmeans[nin+i];
                        if( (double)(s)==(double)(0) )
                        {
                            s = Math.Sign(network.columnsigmas[nin+i]);
                        }
                        if( (double)(s)==(double)(0) )
                        {
                            s = 1.0;
                        }
                        network.columnsigmas[nin+i] = Math.Sign(network.columnsigmas[nin+i])*Math.Abs(s);
                        if( (double)(network.columnsigmas[nin+i])==(double)(0) )
                        {
                            network.columnsigmas[nin+i] = 1;
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Returns information about initialized network: number of inputs, outputs,
        weights.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpproperties(multilayerperceptron network,
            ref int nin,
            ref int nout,
            ref int wcount)
        {
            nin = 0;
            nout = 0;
            wcount = 0;

            nin = network.structinfo[1];
            nout = network.structinfo[2];
            wcount = network.structinfo[4];
        }


        /*************************************************************************
        Tells whether network is SOFTMAX-normalized (i.e. classifier) or not.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static bool mlpissoftmax(multilayerperceptron network)
        {
            bool result = new bool();

            result = network.structinfo[6]==1;
            return result;
        }


        /*************************************************************************
        Procesing

        INPUT PARAMETERS:
            Network -   neural network
            X       -   input vector,  array[0..NIn-1].

        OUTPUT PARAMETERS:
            Y       -   result. Regression estimate when solving regression  task,
                        vector of posterior probabilities for classification task.

        See also MLPProcessI

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpprocess(multilayerperceptron network,
            double[] x,
            ref double[] y)
        {
            if( ap.len(y)<network.structinfo[2] )
            {
                y = new double[network.structinfo[2]];
            }
            mlpinternalprocessvector(network.structinfo, network.weights, network.columnmeans, network.columnsigmas, ref network.neurons, ref network.dfdnet, x, ref y);
        }


        /*************************************************************************
        'interactive'  variant  of  MLPProcess  for  languages  like  Python which
        support constructs like "Y = MLPProcess(NN,X)" and interactive mode of the
        interpreter

        This function allocates new array on each call,  so  it  is  significantly
        slower than its 'non-interactive' counterpart, but it is  more  convenient
        when you call it from command line.

          -- ALGLIB --
             Copyright 21.09.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpprocessi(multilayerperceptron network,
            double[] x,
            ref double[] y)
        {
            y = new double[0];

            mlpprocess(network, x, ref y);
        }


        /*************************************************************************
        Error function for neural network, internal subroutine.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static double mlperror(multilayerperceptron network,
            double[,] xy,
            int ssize)
        {
            double result = 0;
            int i = 0;
            int k = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            double e = 0;
            int i_ = 0;
            int i1_ = 0;

            mlpproperties(network, ref nin, ref nout, ref wcount);
            result = 0;
            for(i=0; i<=ssize-1; i++)
            {
                for(i_=0; i_<=nin-1;i_++)
                {
                    network.x[i_] = xy[i,i_];
                }
                mlpprocess(network, network.x, ref network.y);
                if( mlpissoftmax(network) )
                {
                    
                    //
                    // class labels outputs
                    //
                    k = (int)Math.Round(xy[i,nin]);
                    if( k>=0 & k<nout )
                    {
                        network.y[k] = network.y[k]-1;
                    }
                }
                else
                {
                    
                    //
                    // real outputs
                    //
                    i1_ = (nin) - (0);
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        network.y[i_] = network.y[i_] - xy[i,i_+i1_];
                    }
                }
                e = 0.0;
                for(i_=0; i_<=nout-1;i_++)
                {
                    e += network.y[i_]*network.y[i_];
                }
                result = result+e/2;
            }
            return result;
        }


        /*************************************************************************
        Natural error function for neural network, internal subroutine.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static double mlperrorn(multilayerperceptron network,
            double[,] xy,
            int ssize)
        {
            double result = 0;
            int i = 0;
            int k = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            double e = 0;
            int i_ = 0;
            int i1_ = 0;

            mlpproperties(network, ref nin, ref nout, ref wcount);
            result = 0;
            for(i=0; i<=ssize-1; i++)
            {
                
                //
                // Process vector
                //
                for(i_=0; i_<=nin-1;i_++)
                {
                    network.x[i_] = xy[i,i_];
                }
                mlpprocess(network, network.x, ref network.y);
                
                //
                // Update error function
                //
                if( network.structinfo[6]==0 )
                {
                    
                    //
                    // Least squares error function
                    //
                    i1_ = (nin) - (0);
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        network.y[i_] = network.y[i_] - xy[i,i_+i1_];
                    }
                    e = 0.0;
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        e += network.y[i_]*network.y[i_];
                    }
                    result = result+e/2;
                }
                else
                {
                    
                    //
                    // Cross-entropy error function
                    //
                    k = (int)Math.Round(xy[i,nin]);
                    if( k>=0 & k<nout )
                    {
                        result = result+safecrossentropy(1, network.y[k]);
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Classification error

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static int mlpclserror(multilayerperceptron network,
            double[,] xy,
            int ssize)
        {
            int result = 0;
            int i = 0;
            int j = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            double[] workx = new double[0];
            double[] worky = new double[0];
            int nn = 0;
            int ns = 0;
            int nmax = 0;
            int i_ = 0;

            mlpproperties(network, ref nin, ref nout, ref wcount);
            workx = new double[nin-1+1];
            worky = new double[nout-1+1];
            result = 0;
            for(i=0; i<=ssize-1; i++)
            {
                
                //
                // Process
                //
                for(i_=0; i_<=nin-1;i_++)
                {
                    workx[i_] = xy[i,i_];
                }
                mlpprocess(network, workx, ref worky);
                
                //
                // Network version of the answer
                //
                nmax = 0;
                for(j=0; j<=nout-1; j++)
                {
                    if( (double)(worky[j])>(double)(worky[nmax]) )
                    {
                        nmax = j;
                    }
                }
                nn = nmax;
                
                //
                // Right answer
                //
                if( mlpissoftmax(network) )
                {
                    ns = (int)Math.Round(xy[i,nin]);
                }
                else
                {
                    nmax = 0;
                    for(j=0; j<=nout-1; j++)
                    {
                        if( (double)(xy[i,nin+j])>(double)(xy[i,nin+nmax]) )
                        {
                            nmax = j;
                        }
                    }
                    ns = nmax;
                }
                
                //
                // compare
                //
                if( nn!=ns )
                {
                    result = result+1;
                }
            }
            return result;
        }


        /*************************************************************************
        Relative classification error on the test set

        INPUT PARAMETERS:
            Network -   network
            XY      -   test set
            NPoints -   test set size

        RESULT:
            percent of incorrectly classified cases. Works both for
            classifier networks and general purpose networks used as
            classifiers.

          -- ALGLIB --
             Copyright 25.12.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mlprelclserror(multilayerperceptron network,
            double[,] xy,
            int npoints)
        {
            double result = 0;

            result = (double)mlpclserror(network, xy, npoints)/(double)npoints;
            return result;
        }


        /*************************************************************************
        Average cross-entropy (in bits per element) on the test set

        INPUT PARAMETERS:
            Network -   neural network
            XY      -   test set
            NPoints -   test set size

        RESULT:
            CrossEntropy/(NPoints*LN(2)).
            Zero if network solves regression task.

          -- ALGLIB --
             Copyright 08.01.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double mlpavgce(multilayerperceptron network,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;

            if( mlpissoftmax(network) )
            {
                mlpproperties(network, ref nin, ref nout, ref wcount);
                result = mlperrorn(network, xy, npoints)/(npoints*Math.Log(2));
            }
            else
            {
                result = 0;
            }
            return result;
        }


        /*************************************************************************
        RMS error on the test set

        INPUT PARAMETERS:
            Network -   neural network
            XY      -   test set
            NPoints -   test set size

        RESULT:
            root mean square error.
            Its meaning for regression task is obvious. As for
            classification task, RMS error means error when estimating posterior
            probabilities.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static double mlprmserror(multilayerperceptron network,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;

            mlpproperties(network, ref nin, ref nout, ref wcount);
            result = Math.Sqrt(2*mlperror(network, xy, npoints)/(npoints*nout));
            return result;
        }


        /*************************************************************************
        Average error on the test set

        INPUT PARAMETERS:
            Network -   neural network
            XY      -   test set
            NPoints -   test set size

        RESULT:
            Its meaning for regression task is obvious. As for
            classification task, it means average error when estimating posterior
            probabilities.

          -- ALGLIB --
             Copyright 11.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mlpavgerror(multilayerperceptron network,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int i_ = 0;

            mlpproperties(network, ref nin, ref nout, ref wcount);
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nin-1;i_++)
                {
                    network.x[i_] = xy[i,i_];
                }
                mlpprocess(network, network.x, ref network.y);
                if( mlpissoftmax(network) )
                {
                    
                    //
                    // class labels
                    //
                    k = (int)Math.Round(xy[i,nin]);
                    for(j=0; j<=nout-1; j++)
                    {
                        if( j==k )
                        {
                            result = result+Math.Abs(1-network.y[j]);
                        }
                        else
                        {
                            result = result+Math.Abs(network.y[j]);
                        }
                    }
                }
                else
                {
                    
                    //
                    // real outputs
                    //
                    for(j=0; j<=nout-1; j++)
                    {
                        result = result+Math.Abs(xy[i,nin+j]-network.y[j]);
                    }
                }
            }
            result = result/(npoints*nout);
            return result;
        }


        /*************************************************************************
        Average relative error on the test set

        INPUT PARAMETERS:
            Network -   neural network
            XY      -   test set
            NPoints -   test set size

        RESULT:
            Its meaning for regression task is obvious. As for
            classification task, it means average relative error when estimating
            posterior probability of belonging to the correct class.

          -- ALGLIB --
             Copyright 11.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mlpavgrelerror(multilayerperceptron network,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int lk = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int i_ = 0;

            mlpproperties(network, ref nin, ref nout, ref wcount);
            result = 0;
            k = 0;
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nin-1;i_++)
                {
                    network.x[i_] = xy[i,i_];
                }
                mlpprocess(network, network.x, ref network.y);
                if( mlpissoftmax(network) )
                {
                    
                    //
                    // class labels
                    //
                    lk = (int)Math.Round(xy[i,nin]);
                    for(j=0; j<=nout-1; j++)
                    {
                        if( j==lk )
                        {
                            result = result+Math.Abs(1-network.y[j]);
                            k = k+1;
                        }
                    }
                }
                else
                {
                    
                    //
                    // real outputs
                    //
                    for(j=0; j<=nout-1; j++)
                    {
                        if( (double)(xy[i,nin+j])!=(double)(0) )
                        {
                            result = result+Math.Abs(xy[i,nin+j]-network.y[j])/Math.Abs(xy[i,nin+j]);
                            k = k+1;
                        }
                    }
                }
            }
            if( k!=0 )
            {
                result = result/k;
            }
            return result;
        }


        /*************************************************************************
        Gradient calculation

        INPUT PARAMETERS:
            Network -   network initialized with one of the network creation funcs
            X       -   input vector, length of array must be at least NIn
            DesiredY-   desired outputs, length of array must be at least NOut
            Grad    -   possibly preallocated array. If size of array is smaller
                        than WCount, it will be reallocated. It is recommended to
                        reuse previously allocated array to reduce allocation
                        overhead.

        OUTPUT PARAMETERS:
            E       -   error function, SUM(sqr(y[i]-desiredy[i])/2,i)
            Grad    -   gradient of E with respect to weights of network, array[WCount]
            
          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpgrad(multilayerperceptron network,
            double[] x,
            double[] desiredy,
            ref double e,
            ref double[] grad)
        {
            int i = 0;
            int nout = 0;
            int ntotal = 0;

            e = 0;

            
            //
            // Alloc
            //
            if( ap.len(grad)<network.structinfo[4] )
            {
                grad = new double[network.structinfo[4]];
            }
            
            //
            // Prepare dError/dOut, internal structures
            //
            mlpprocess(network, x, ref network.y);
            nout = network.structinfo[2];
            ntotal = network.structinfo[3];
            e = 0;
            for(i=0; i<=ntotal-1; i++)
            {
                network.derror[i] = 0;
            }
            for(i=0; i<=nout-1; i++)
            {
                network.derror[ntotal-nout+i] = network.y[i]-desiredy[i];
                e = e+math.sqr(network.y[i]-desiredy[i])/2;
            }
            
            //
            // gradient
            //
            mlpinternalcalculategradient(network, network.neurons, network.weights, ref network.derror, ref grad, false);
        }


        /*************************************************************************
        Gradient calculation (natural error function is used)

        INPUT PARAMETERS:
            Network -   network initialized with one of the network creation funcs
            X       -   input vector, length of array must be at least NIn
            DesiredY-   desired outputs, length of array must be at least NOut
            Grad    -   possibly preallocated array. If size of array is smaller
                        than WCount, it will be reallocated. It is recommended to
                        reuse previously allocated array to reduce allocation
                        overhead.

        OUTPUT PARAMETERS:
            E       -   error function, sum-of-squares for regression networks,
                        cross-entropy for classification networks.
            Grad    -   gradient of E with respect to weights of network, array[WCount]

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpgradn(multilayerperceptron network,
            double[] x,
            double[] desiredy,
            ref double e,
            ref double[] grad)
        {
            double s = 0;
            int i = 0;
            int nout = 0;
            int ntotal = 0;

            e = 0;

            
            //
            // Alloc
            //
            if( ap.len(grad)<network.structinfo[4] )
            {
                grad = new double[network.structinfo[4]];
            }
            
            //
            // Prepare dError/dOut, internal structures
            //
            mlpprocess(network, x, ref network.y);
            nout = network.structinfo[2];
            ntotal = network.structinfo[3];
            for(i=0; i<=ntotal-1; i++)
            {
                network.derror[i] = 0;
            }
            e = 0;
            if( network.structinfo[6]==0 )
            {
                
                //
                // Regression network, least squares
                //
                for(i=0; i<=nout-1; i++)
                {
                    network.derror[ntotal-nout+i] = network.y[i]-desiredy[i];
                    e = e+math.sqr(network.y[i]-desiredy[i])/2;
                }
            }
            else
            {
                
                //
                // Classification network, cross-entropy
                //
                s = 0;
                for(i=0; i<=nout-1; i++)
                {
                    s = s+desiredy[i];
                }
                for(i=0; i<=nout-1; i++)
                {
                    network.derror[ntotal-nout+i] = s*network.y[i]-desiredy[i];
                    e = e+safecrossentropy(desiredy[i], network.y[i]);
                }
            }
            
            //
            // gradient
            //
            mlpinternalcalculategradient(network, network.neurons, network.weights, ref network.derror, ref grad, true);
        }


        /*************************************************************************
        Batch gradient calculation for a set of inputs/outputs

        INPUT PARAMETERS:
            Network -   network initialized with one of the network creation funcs
            XY      -   set of inputs/outputs; one sample = one row;
                        first NIn columns contain inputs,
                        next NOut columns - desired outputs.
            SSize   -   number of elements in XY
            Grad    -   possibly preallocated array. If size of array is smaller
                        than WCount, it will be reallocated. It is recommended to
                        reuse previously allocated array to reduce allocation
                        overhead.

        OUTPUT PARAMETERS:
            E       -   error function, SUM(sqr(y[i]-desiredy[i])/2,i)
            Grad    -   gradient of E with respect to weights of network, array[WCount]

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpgradbatch(multilayerperceptron network,
            double[,] xy,
            int ssize,
            ref double e,
            ref double[] grad)
        {
            int i = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;

            e = 0;

            mlpproperties(network, ref nin, ref nout, ref wcount);
            for(i=0; i<=wcount-1; i++)
            {
                grad[i] = 0;
            }
            e = 0;
            i = 0;
            while( i<=ssize-1 )
            {
                mlpchunkedgradient(network, xy, i, Math.Min(ssize, i+chunksize)-i, ref e, ref grad, false);
                i = i+chunksize;
            }
        }


        /*************************************************************************
        Batch gradient calculation for a set of inputs/outputs
        (natural error function is used)

        INPUT PARAMETERS:
            Network -   network initialized with one of the network creation funcs
            XY      -   set of inputs/outputs; one sample = one row;
                        first NIn columns contain inputs,
                        next NOut columns - desired outputs.
            SSize   -   number of elements in XY
            Grad    -   possibly preallocated array. If size of array is smaller
                        than WCount, it will be reallocated. It is recommended to
                        reuse previously allocated array to reduce allocation
                        overhead.

        OUTPUT PARAMETERS:
            E       -   error function, sum-of-squares for regression networks,
                        cross-entropy for classification networks.
            Grad    -   gradient of E with respect to weights of network, array[WCount]

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpgradnbatch(multilayerperceptron network,
            double[,] xy,
            int ssize,
            ref double e,
            ref double[] grad)
        {
            int i = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;

            e = 0;

            mlpproperties(network, ref nin, ref nout, ref wcount);
            for(i=0; i<=wcount-1; i++)
            {
                grad[i] = 0;
            }
            e = 0;
            i = 0;
            while( i<=ssize-1 )
            {
                mlpchunkedgradient(network, xy, i, Math.Min(ssize, i+chunksize)-i, ref e, ref grad, true);
                i = i+chunksize;
            }
        }


        /*************************************************************************
        Batch Hessian calculation (natural error function) using R-algorithm.
        Internal subroutine.

          -- ALGLIB --
             Copyright 26.01.2008 by Bochkanov Sergey.
             
             Hessian calculation based on R-algorithm described in
             "Fast Exact Multiplication by the Hessian",
             B. A. Pearlmutter,
             Neural Computation, 1994.
        *************************************************************************/
        public static void mlphessiannbatch(multilayerperceptron network,
            double[,] xy,
            int ssize,
            ref double e,
            ref double[] grad,
            ref double[,] h)
        {
            e = 0;

            mlphessianbatchinternal(network, xy, ssize, true, ref e, ref grad, ref h);
        }


        /*************************************************************************
        Batch Hessian calculation using R-algorithm.
        Internal subroutine.

          -- ALGLIB --
             Copyright 26.01.2008 by Bochkanov Sergey.

             Hessian calculation based on R-algorithm described in
             "Fast Exact Multiplication by the Hessian",
             B. A. Pearlmutter,
             Neural Computation, 1994.
        *************************************************************************/
        public static void mlphessianbatch(multilayerperceptron network,
            double[,] xy,
            int ssize,
            ref double e,
            ref double[] grad,
            ref double[,] h)
        {
            e = 0;

            mlphessianbatchinternal(network, xy, ssize, false, ref e, ref grad, ref h);
        }


        /*************************************************************************
        Internal subroutine, shouldn't be called by user.
        *************************************************************************/
        public static void mlpinternalprocessvector(int[] structinfo,
            double[] weights,
            double[] columnmeans,
            double[] columnsigmas,
            ref double[] neurons,
            ref double[] dfdnet,
            double[] x,
            ref double[] y)
        {
            int i = 0;
            int n1 = 0;
            int n2 = 0;
            int w1 = 0;
            int w2 = 0;
            int ntotal = 0;
            int nin = 0;
            int nout = 0;
            int istart = 0;
            int offs = 0;
            double net = 0;
            double f = 0;
            double df = 0;
            double d2f = 0;
            double mx = 0;
            bool perr = new bool();
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Read network geometry
            //
            nin = structinfo[1];
            nout = structinfo[2];
            ntotal = structinfo[3];
            istart = structinfo[5];
            
            //
            // Inputs standartisation and putting in the network
            //
            for(i=0; i<=nin-1; i++)
            {
                if( (double)(columnsigmas[i])!=(double)(0) )
                {
                    neurons[i] = (x[i]-columnmeans[i])/columnsigmas[i];
                }
                else
                {
                    neurons[i] = x[i]-columnmeans[i];
                }
            }
            
            //
            // Process network
            //
            for(i=0; i<=ntotal-1; i++)
            {
                offs = istart+i*nfieldwidth;
                if( structinfo[offs+0]>0 )
                {
                    
                    //
                    // Activation function
                    //
                    mlpactivationfunction(neurons[structinfo[offs+2]], structinfo[offs+0], ref f, ref df, ref d2f);
                    neurons[i] = f;
                    dfdnet[i] = df;
                }
                if( structinfo[offs+0]==0 )
                {
                    
                    //
                    // Adaptive summator
                    //
                    n1 = structinfo[offs+2];
                    n2 = n1+structinfo[offs+1]-1;
                    w1 = structinfo[offs+3];
                    w2 = w1+structinfo[offs+1]-1;
                    i1_ = (n1)-(w1);
                    net = 0.0;
                    for(i_=w1; i_<=w2;i_++)
                    {
                        net += weights[i_]*neurons[i_+i1_];
                    }
                    neurons[i] = net;
                    dfdnet[i] = 1.0;
                }
                if( structinfo[offs+0]<0 )
                {
                    perr = true;
                    if( structinfo[offs+0]==-2 )
                    {
                        
                        //
                        // input neuron, left unchanged
                        //
                        perr = false;
                    }
                    if( structinfo[offs+0]==-3 )
                    {
                        
                        //
                        // "-1" neuron
                        //
                        neurons[i] = -1;
                        perr = false;
                    }
                    if( structinfo[offs+0]==-4 )
                    {
                        
                        //
                        // "0" neuron
                        //
                        neurons[i] = 0;
                        perr = false;
                    }
                    ap.assert(!perr, "MLPInternalProcessVector: internal error - unknown neuron type!");
                }
            }
            
            //
            // Extract result
            //
            i1_ = (ntotal-nout) - (0);
            for(i_=0; i_<=nout-1;i_++)
            {
                y[i_] = neurons[i_+i1_];
            }
            
            //
            // Softmax post-processing or standardisation if needed
            //
            ap.assert(structinfo[6]==0 | structinfo[6]==1, "MLPInternalProcessVector: unknown normalization type!");
            if( structinfo[6]==1 )
            {
                
                //
                // Softmax
                //
                mx = y[0];
                for(i=1; i<=nout-1; i++)
                {
                    mx = Math.Max(mx, y[i]);
                }
                net = 0;
                for(i=0; i<=nout-1; i++)
                {
                    y[i] = Math.Exp(y[i]-mx);
                    net = net+y[i];
                }
                for(i=0; i<=nout-1; i++)
                {
                    y[i] = y[i]/net;
                }
            }
            else
            {
                
                //
                // Standardisation
                //
                for(i=0; i<=nout-1; i++)
                {
                    y[i] = y[i]*columnsigmas[nin+i]+columnmeans[nin+i];
                }
            }
        }


        /*************************************************************************
        Internal subroutine: adding new input layer to network
        *************************************************************************/
        private static void addinputlayer(int ncount,
            ref int[] lsizes,
            ref int[] ltypes,
            ref int[] lconnfirst,
            ref int[] lconnlast,
            ref int lastproc)
        {
            lsizes[0] = ncount;
            ltypes[0] = -2;
            lconnfirst[0] = 0;
            lconnlast[0] = 0;
            lastproc = 0;
        }


        /*************************************************************************
        Internal subroutine: adding new summator layer to network
        *************************************************************************/
        private static void addbiasedsummatorlayer(int ncount,
            ref int[] lsizes,
            ref int[] ltypes,
            ref int[] lconnfirst,
            ref int[] lconnlast,
            ref int lastproc)
        {
            lsizes[lastproc+1] = 1;
            ltypes[lastproc+1] = -3;
            lconnfirst[lastproc+1] = 0;
            lconnlast[lastproc+1] = 0;
            lsizes[lastproc+2] = ncount;
            ltypes[lastproc+2] = 0;
            lconnfirst[lastproc+2] = lastproc;
            lconnlast[lastproc+2] = lastproc+1;
            lastproc = lastproc+2;
        }


        /*************************************************************************
        Internal subroutine: adding new summator layer to network
        *************************************************************************/
        private static void addactivationlayer(int functype,
            ref int[] lsizes,
            ref int[] ltypes,
            ref int[] lconnfirst,
            ref int[] lconnlast,
            ref int lastproc)
        {
            ap.assert(functype>0, "AddActivationLayer: incorrect function type");
            lsizes[lastproc+1] = lsizes[lastproc];
            ltypes[lastproc+1] = functype;
            lconnfirst[lastproc+1] = lastproc;
            lconnlast[lastproc+1] = lastproc;
            lastproc = lastproc+1;
        }


        /*************************************************************************
        Internal subroutine: adding new zero layer to network
        *************************************************************************/
        private static void addzerolayer(ref int[] lsizes,
            ref int[] ltypes,
            ref int[] lconnfirst,
            ref int[] lconnlast,
            ref int lastproc)
        {
            lsizes[lastproc+1] = 1;
            ltypes[lastproc+1] = -4;
            lconnfirst[lastproc+1] = 0;
            lconnlast[lastproc+1] = 0;
            lastproc = lastproc+1;
        }


        /*************************************************************************
        Internal subroutine.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        private static void mlpcreate(int nin,
            int nout,
            int[] lsizes,
            int[] ltypes,
            int[] lconnfirst,
            int[] lconnlast,
            int layerscount,
            bool isclsnet,
            multilayerperceptron network)
        {
            int i = 0;
            int j = 0;
            int ssize = 0;
            int ntotal = 0;
            int wcount = 0;
            int offs = 0;
            int nprocessed = 0;
            int wallocated = 0;
            int[] localtemp = new int[0];
            int[] lnfirst = new int[0];
            int[] lnsyn = new int[0];

            
            //
            // Check
            //
            ap.assert(layerscount>0, "MLPCreate: wrong parameters!");
            ap.assert(ltypes[0]==-2, "MLPCreate: wrong LTypes[0] (must be -2)!");
            for(i=0; i<=layerscount-1; i++)
            {
                ap.assert(lsizes[i]>0, "MLPCreate: wrong LSizes!");
                ap.assert(lconnfirst[i]>=0 & (lconnfirst[i]<i | i==0), "MLPCreate: wrong LConnFirst!");
                ap.assert(lconnlast[i]>=lconnfirst[i] & (lconnlast[i]<i | i==0), "MLPCreate: wrong LConnLast!");
            }
            
            //
            // Build network geometry
            //
            lnfirst = new int[layerscount-1+1];
            lnsyn = new int[layerscount-1+1];
            ntotal = 0;
            wcount = 0;
            for(i=0; i<=layerscount-1; i++)
            {
                
                //
                // Analyze connections.
                // This code must throw an assertion in case of unknown LTypes[I]
                //
                lnsyn[i] = -1;
                if( ltypes[i]>=0 )
                {
                    lnsyn[i] = 0;
                    for(j=lconnfirst[i]; j<=lconnlast[i]; j++)
                    {
                        lnsyn[i] = lnsyn[i]+lsizes[j];
                    }
                }
                else
                {
                    if( (ltypes[i]==-2 | ltypes[i]==-3) | ltypes[i]==-4 )
                    {
                        lnsyn[i] = 0;
                    }
                }
                ap.assert(lnsyn[i]>=0, "MLPCreate: internal error #0!");
                
                //
                // Other info
                //
                lnfirst[i] = ntotal;
                ntotal = ntotal+lsizes[i];
                if( ltypes[i]==0 )
                {
                    wcount = wcount+lnsyn[i]*lsizes[i];
                }
            }
            ssize = 7+ntotal*nfieldwidth;
            
            //
            // Allocate
            //
            network.structinfo = new int[ssize-1+1];
            network.weights = new double[wcount-1+1];
            if( isclsnet )
            {
                network.columnmeans = new double[nin-1+1];
                network.columnsigmas = new double[nin-1+1];
            }
            else
            {
                network.columnmeans = new double[nin+nout-1+1];
                network.columnsigmas = new double[nin+nout-1+1];
            }
            network.neurons = new double[ntotal-1+1];
            network.chunks = new double[3*ntotal+1, chunksize-1+1];
            network.nwbuf = new double[Math.Max(wcount, 2*nout)-1+1];
            network.dfdnet = new double[ntotal-1+1];
            network.x = new double[nin-1+1];
            network.y = new double[nout-1+1];
            network.derror = new double[ntotal-1+1];
            
            //
            // Fill structure: global info
            //
            network.structinfo[0] = ssize;
            network.structinfo[1] = nin;
            network.structinfo[2] = nout;
            network.structinfo[3] = ntotal;
            network.structinfo[4] = wcount;
            network.structinfo[5] = 7;
            if( isclsnet )
            {
                network.structinfo[6] = 1;
            }
            else
            {
                network.structinfo[6] = 0;
            }
            
            //
            // Fill structure: neuron connections
            //
            nprocessed = 0;
            wallocated = 0;
            for(i=0; i<=layerscount-1; i++)
            {
                for(j=0; j<=lsizes[i]-1; j++)
                {
                    offs = network.structinfo[5]+nprocessed*nfieldwidth;
                    network.structinfo[offs+0] = ltypes[i];
                    if( ltypes[i]==0 )
                    {
                        
                        //
                        // Adaptive summator:
                        // * connections with weights to previous neurons
                        //
                        network.structinfo[offs+1] = lnsyn[i];
                        network.structinfo[offs+2] = lnfirst[lconnfirst[i]];
                        network.structinfo[offs+3] = wallocated;
                        wallocated = wallocated+lnsyn[i];
                        nprocessed = nprocessed+1;
                    }
                    if( ltypes[i]>0 )
                    {
                        
                        //
                        // Activation layer:
                        // * each neuron connected to one (only one) of previous neurons.
                        // * no weights
                        //
                        network.structinfo[offs+1] = 1;
                        network.structinfo[offs+2] = lnfirst[lconnfirst[i]]+j;
                        network.structinfo[offs+3] = -1;
                        nprocessed = nprocessed+1;
                    }
                    if( (ltypes[i]==-2 | ltypes[i]==-3) | ltypes[i]==-4 )
                    {
                        nprocessed = nprocessed+1;
                    }
                }
            }
            ap.assert(wallocated==wcount, "MLPCreate: internal error #1!");
            ap.assert(nprocessed==ntotal, "MLPCreate: internal error #2!");
            
            //
            // Fill weights by small random values
            // Initialize means and sigmas
            //
            for(i=0; i<=wcount-1; i++)
            {
                network.weights[i] = math.randomreal()-0.5;
            }
            for(i=0; i<=nin-1; i++)
            {
                network.columnmeans[i] = 0;
                network.columnsigmas[i] = 1;
            }
            if( !isclsnet )
            {
                for(i=0; i<=nout-1; i++)
                {
                    network.columnmeans[nin+i] = 0;
                    network.columnsigmas[nin+i] = 1;
                }
            }
        }


        /*************************************************************************
        Internal subroutine

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        private static void mlpactivationfunction(double net,
            int k,
            ref double f,
            ref double df,
            ref double d2f)
        {
            double net2 = 0;
            double arg = 0;
            double root = 0;
            double r = 0;

            f = 0;
            df = 0;
            d2f = 0;

            f = 0;
            df = 0;
            if( k==1 )
            {
                
                //
                // TanH activation function
                //
                if( (double)(Math.Abs(net))<(double)(100) )
                {
                    f = Math.Tanh(net);
                }
                else
                {
                    f = Math.Sign(net);
                }
                df = 1-math.sqr(f);
                d2f = -(2*f*df);
                return;
            }
            if( k==3 )
            {
                
                //
                // EX activation function
                //
                if( (double)(net)>=(double)(0) )
                {
                    net2 = net*net;
                    arg = net2+1;
                    root = Math.Sqrt(arg);
                    f = net+root;
                    r = net/root;
                    df = 1+r;
                    d2f = (root-net*r)/arg;
                }
                else
                {
                    f = Math.Exp(net);
                    df = f;
                    d2f = f;
                }
                return;
            }
            if( k==2 )
            {
                f = Math.Exp(-math.sqr(net));
                df = -(2*net*f);
                d2f = -(2*(f+df*net));
                return;
            }
        }


        /*************************************************************************
        Internal subroutine for Hessian calculation.

        WARNING!!! Unspeakable math far beyong human capabilities :)
        *************************************************************************/
        private static void mlphessianbatchinternal(multilayerperceptron network,
            double[,] xy,
            int ssize,
            bool naturalerr,
            ref double e,
            ref double[] grad,
            ref double[,] h)
        {
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int ntotal = 0;
            int istart = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int kl = 0;
            int offs = 0;
            int n1 = 0;
            int n2 = 0;
            int w1 = 0;
            int w2 = 0;
            double s = 0;
            double t = 0;
            double v = 0;
            double et = 0;
            bool bflag = new bool();
            double f = 0;
            double df = 0;
            double d2f = 0;
            double deidyj = 0;
            double mx = 0;
            double q = 0;
            double z = 0;
            double s2 = 0;
            double expi = 0;
            double expj = 0;
            double[] x = new double[0];
            double[] desiredy = new double[0];
            double[] gt = new double[0];
            double[] zeros = new double[0];
            double[,] rx = new double[0,0];
            double[,] ry = new double[0,0];
            double[,] rdx = new double[0,0];
            double[,] rdy = new double[0,0];
            int i_ = 0;
            int i1_ = 0;

            e = 0;

            mlpproperties(network, ref nin, ref nout, ref wcount);
            ntotal = network.structinfo[3];
            istart = network.structinfo[5];
            
            //
            // Prepare
            //
            x = new double[nin-1+1];
            desiredy = new double[nout-1+1];
            zeros = new double[wcount-1+1];
            gt = new double[wcount-1+1];
            rx = new double[ntotal+nout-1+1, wcount-1+1];
            ry = new double[ntotal+nout-1+1, wcount-1+1];
            rdx = new double[ntotal+nout-1+1, wcount-1+1];
            rdy = new double[ntotal+nout-1+1, wcount-1+1];
            e = 0;
            for(i=0; i<=wcount-1; i++)
            {
                zeros[i] = 0;
            }
            for(i_=0; i_<=wcount-1;i_++)
            {
                grad[i_] = zeros[i_];
            }
            for(i=0; i<=wcount-1; i++)
            {
                for(i_=0; i_<=wcount-1;i_++)
                {
                    h[i,i_] = zeros[i_];
                }
            }
            
            //
            // Process
            //
            for(k=0; k<=ssize-1; k++)
            {
                
                //
                // Process vector with MLPGradN.
                // Now Neurons, DFDNET and DError contains results of the last run.
                //
                for(i_=0; i_<=nin-1;i_++)
                {
                    x[i_] = xy[k,i_];
                }
                if( mlpissoftmax(network) )
                {
                    
                    //
                    // class labels outputs
                    //
                    kl = (int)Math.Round(xy[k,nin]);
                    for(i=0; i<=nout-1; i++)
                    {
                        if( i==kl )
                        {
                            desiredy[i] = 1;
                        }
                        else
                        {
                            desiredy[i] = 0;
                        }
                    }
                }
                else
                {
                    
                    //
                    // real outputs
                    //
                    i1_ = (nin) - (0);
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        desiredy[i_] = xy[k,i_+i1_];
                    }
                }
                if( naturalerr )
                {
                    mlpgradn(network, x, desiredy, ref et, ref gt);
                }
                else
                {
                    mlpgrad(network, x, desiredy, ref et, ref gt);
                }
                
                //
                // grad, error
                //
                e = e+et;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    grad[i_] = grad[i_] + gt[i_];
                }
                
                //
                // Hessian.
                // Forward pass of the R-algorithm
                //
                for(i=0; i<=ntotal-1; i++)
                {
                    offs = istart+i*nfieldwidth;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        rx[i,i_] = zeros[i_];
                    }
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        ry[i,i_] = zeros[i_];
                    }
                    if( network.structinfo[offs+0]>0 )
                    {
                        
                        //
                        // Activation function
                        //
                        n1 = network.structinfo[offs+2];
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rx[i,i_] = ry[n1,i_];
                        }
                        v = network.dfdnet[i];
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            ry[i,i_] = v*rx[i,i_];
                        }
                    }
                    if( network.structinfo[offs+0]==0 )
                    {
                        
                        //
                        // Adaptive summator
                        //
                        n1 = network.structinfo[offs+2];
                        n2 = n1+network.structinfo[offs+1]-1;
                        w1 = network.structinfo[offs+3];
                        w2 = w1+network.structinfo[offs+1]-1;
                        for(j=n1; j<=n2; j++)
                        {
                            v = network.weights[w1+j-n1];
                            for(i_=0; i_<=wcount-1;i_++)
                            {
                                rx[i,i_] = rx[i,i_] + v*ry[j,i_];
                            }
                            rx[i,w1+j-n1] = rx[i,w1+j-n1]+network.neurons[j];
                        }
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            ry[i,i_] = rx[i,i_];
                        }
                    }
                    if( network.structinfo[offs+0]<0 )
                    {
                        bflag = true;
                        if( network.structinfo[offs+0]==-2 )
                        {
                            
                            //
                            // input neuron, left unchanged
                            //
                            bflag = false;
                        }
                        if( network.structinfo[offs+0]==-3 )
                        {
                            
                            //
                            // "-1" neuron, left unchanged
                            //
                            bflag = false;
                        }
                        if( network.structinfo[offs+0]==-4 )
                        {
                            
                            //
                            // "0" neuron, left unchanged
                            //
                            bflag = false;
                        }
                        ap.assert(!bflag, "MLPHessianNBatch: internal error - unknown neuron type!");
                    }
                }
                
                //
                // Hessian. Backward pass of the R-algorithm.
                //
                // Stage 1. Initialize RDY
                //
                for(i=0; i<=ntotal+nout-1; i++)
                {
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        rdy[i,i_] = zeros[i_];
                    }
                }
                if( network.structinfo[6]==0 )
                {
                    
                    //
                    // Standardisation.
                    //
                    // In context of the Hessian calculation standardisation
                    // is considered as additional layer with weightless
                    // activation function:
                    //
                    // F(NET) := Sigma*NET
                    //
                    // So we add one more layer to forward pass, and
                    // make forward/backward pass through this layer.
                    //
                    for(i=0; i<=nout-1; i++)
                    {
                        n1 = ntotal-nout+i;
                        n2 = ntotal+i;
                        
                        //
                        // Forward pass from N1 to N2
                        //
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rx[n2,i_] = ry[n1,i_];
                        }
                        v = network.columnsigmas[nin+i];
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            ry[n2,i_] = v*rx[n2,i_];
                        }
                        
                        //
                        // Initialization of RDY
                        //
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rdy[n2,i_] = ry[n2,i_];
                        }
                        
                        //
                        // Backward pass from N2 to N1:
                        // 1. Calculate R(dE/dX).
                        // 2. No R(dE/dWij) is needed since weight of activation neuron
                        //    is fixed to 1. So we can update R(dE/dY) for
                        //    the connected neuron (note that Vij=0, Wij=1)
                        //
                        df = network.columnsigmas[nin+i];
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rdx[n2,i_] = df*rdy[n2,i_];
                        }
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rdy[n1,i_] = rdy[n1,i_] + rdx[n2,i_];
                        }
                    }
                }
                else
                {
                    
                    //
                    // Softmax.
                    //
                    // Initialize RDY using generalized expression for ei'(yi)
                    // (see expression (9) from p. 5 of "Fast Exact Multiplication by the Hessian").
                    //
                    // When we are working with softmax network, generalized
                    // expression for ei'(yi) is used because softmax
                    // normalization leads to ei, which depends on all y's
                    //
                    if( naturalerr )
                    {
                        
                        //
                        // softmax + cross-entropy.
                        // We have:
                        //
                        // S = sum(exp(yk)),
                        // ei = sum(trn)*exp(yi)/S-trn_i
                        //
                        // j=i:   d(ei)/d(yj) = T*exp(yi)*(S-exp(yi))/S^2
                        // j<>i:  d(ei)/d(yj) = -T*exp(yi)*exp(yj)/S^2
                        //
                        t = 0;
                        for(i=0; i<=nout-1; i++)
                        {
                            t = t+desiredy[i];
                        }
                        mx = network.neurons[ntotal-nout];
                        for(i=0; i<=nout-1; i++)
                        {
                            mx = Math.Max(mx, network.neurons[ntotal-nout+i]);
                        }
                        s = 0;
                        for(i=0; i<=nout-1; i++)
                        {
                            network.nwbuf[i] = Math.Exp(network.neurons[ntotal-nout+i]-mx);
                            s = s+network.nwbuf[i];
                        }
                        for(i=0; i<=nout-1; i++)
                        {
                            for(j=0; j<=nout-1; j++)
                            {
                                if( j==i )
                                {
                                    deidyj = t*network.nwbuf[i]*(s-network.nwbuf[i])/math.sqr(s);
                                    for(i_=0; i_<=wcount-1;i_++)
                                    {
                                        rdy[ntotal-nout+i,i_] = rdy[ntotal-nout+i,i_] + deidyj*ry[ntotal-nout+i,i_];
                                    }
                                }
                                else
                                {
                                    deidyj = -(t*network.nwbuf[i]*network.nwbuf[j]/math.sqr(s));
                                    for(i_=0; i_<=wcount-1;i_++)
                                    {
                                        rdy[ntotal-nout+i,i_] = rdy[ntotal-nout+i,i_] + deidyj*ry[ntotal-nout+j,i_];
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        
                        //
                        // For a softmax + squared error we have expression
                        // far beyond human imagination so we dont even try
                        // to comment on it. Just enjoy the code...
                        //
                        // P.S. That's why "natural error" is called "natural" -
                        // compact beatiful expressions, fast code....
                        //
                        mx = network.neurons[ntotal-nout];
                        for(i=0; i<=nout-1; i++)
                        {
                            mx = Math.Max(mx, network.neurons[ntotal-nout+i]);
                        }
                        s = 0;
                        s2 = 0;
                        for(i=0; i<=nout-1; i++)
                        {
                            network.nwbuf[i] = Math.Exp(network.neurons[ntotal-nout+i]-mx);
                            s = s+network.nwbuf[i];
                            s2 = s2+math.sqr(network.nwbuf[i]);
                        }
                        q = 0;
                        for(i=0; i<=nout-1; i++)
                        {
                            q = q+(network.y[i]-desiredy[i])*network.nwbuf[i];
                        }
                        for(i=0; i<=nout-1; i++)
                        {
                            z = -q+(network.y[i]-desiredy[i])*s;
                            expi = network.nwbuf[i];
                            for(j=0; j<=nout-1; j++)
                            {
                                expj = network.nwbuf[j];
                                if( j==i )
                                {
                                    deidyj = expi/math.sqr(s)*((z+expi)*(s-2*expi)/s+expi*s2/math.sqr(s));
                                }
                                else
                                {
                                    deidyj = expi*expj/math.sqr(s)*(s2/math.sqr(s)-2*z/s-(expi+expj)/s+(network.y[i]-desiredy[i])-(network.y[j]-desiredy[j]));
                                }
                                for(i_=0; i_<=wcount-1;i_++)
                                {
                                    rdy[ntotal-nout+i,i_] = rdy[ntotal-nout+i,i_] + deidyj*ry[ntotal-nout+j,i_];
                                }
                            }
                        }
                    }
                }
                
                //
                // Hessian. Backward pass of the R-algorithm
                //
                // Stage 2. Process.
                //
                for(i=ntotal-1; i>=0; i--)
                {
                    
                    //
                    // Possible variants:
                    // 1. Activation function
                    // 2. Adaptive summator
                    // 3. Special neuron
                    //
                    offs = istart+i*nfieldwidth;
                    if( network.structinfo[offs+0]>0 )
                    {
                        n1 = network.structinfo[offs+2];
                        
                        //
                        // First, calculate R(dE/dX).
                        //
                        mlpactivationfunction(network.neurons[n1], network.structinfo[offs+0], ref f, ref df, ref d2f);
                        v = d2f*network.derror[i];
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rdx[i,i_] = df*rdy[i,i_];
                        }
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rdx[i,i_] = rdx[i,i_] + v*rx[i,i_];
                        }
                        
                        //
                        // No R(dE/dWij) is needed since weight of activation neuron
                        // is fixed to 1.
                        //
                        // So we can update R(dE/dY) for the connected neuron.
                        // (note that Vij=0, Wij=1)
                        //
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rdy[n1,i_] = rdy[n1,i_] + rdx[i,i_];
                        }
                    }
                    if( network.structinfo[offs+0]==0 )
                    {
                        
                        //
                        // Adaptive summator
                        //
                        n1 = network.structinfo[offs+2];
                        n2 = n1+network.structinfo[offs+1]-1;
                        w1 = network.structinfo[offs+3];
                        w2 = w1+network.structinfo[offs+1]-1;
                        
                        //
                        // First, calculate R(dE/dX).
                        //
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rdx[i,i_] = rdy[i,i_];
                        }
                        
                        //
                        // Then, calculate R(dE/dWij)
                        //
                        for(j=w1; j<=w2; j++)
                        {
                            v = network.neurons[n1+j-w1];
                            for(i_=0; i_<=wcount-1;i_++)
                            {
                                h[j,i_] = h[j,i_] + v*rdx[i,i_];
                            }
                            v = network.derror[i];
                            for(i_=0; i_<=wcount-1;i_++)
                            {
                                h[j,i_] = h[j,i_] + v*ry[n1+j-w1,i_];
                            }
                        }
                        
                        //
                        // And finally, update R(dE/dY) for connected neurons.
                        //
                        for(j=w1; j<=w2; j++)
                        {
                            v = network.weights[j];
                            for(i_=0; i_<=wcount-1;i_++)
                            {
                                rdy[n1+j-w1,i_] = rdy[n1+j-w1,i_] + v*rdx[i,i_];
                            }
                            rdy[n1+j-w1,j] = rdy[n1+j-w1,j]+network.derror[i];
                        }
                    }
                    if( network.structinfo[offs+0]<0 )
                    {
                        bflag = false;
                        if( (network.structinfo[offs+0]==-2 | network.structinfo[offs+0]==-3) | network.structinfo[offs+0]==-4 )
                        {
                            
                            //
                            // Special neuron type, no back-propagation required
                            //
                            bflag = true;
                        }
                        ap.assert(bflag, "MLPHessianNBatch: unknown neuron type!");
                    }
                }
            }
        }


        /*************************************************************************
        Internal subroutine

        Network must be processed by MLPProcess on X
        *************************************************************************/
        private static void mlpinternalcalculategradient(multilayerperceptron network,
            double[] neurons,
            double[] weights,
            ref double[] derror,
            ref double[] grad,
            bool naturalerrorfunc)
        {
            int i = 0;
            int n1 = 0;
            int n2 = 0;
            int w1 = 0;
            int w2 = 0;
            int ntotal = 0;
            int istart = 0;
            int nin = 0;
            int nout = 0;
            int offs = 0;
            double dedf = 0;
            double dfdnet = 0;
            double v = 0;
            double fown = 0;
            double deown = 0;
            double net = 0;
            double mx = 0;
            bool bflag = new bool();
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Read network geometry
            //
            nin = network.structinfo[1];
            nout = network.structinfo[2];
            ntotal = network.structinfo[3];
            istart = network.structinfo[5];
            
            //
            // Pre-processing of dError/dOut:
            // from dError/dOut(normalized) to dError/dOut(non-normalized)
            //
            ap.assert(network.structinfo[6]==0 | network.structinfo[6]==1, "MLPInternalCalculateGradient: unknown normalization type!");
            if( network.structinfo[6]==1 )
            {
                
                //
                // Softmax
                //
                if( !naturalerrorfunc )
                {
                    mx = network.neurons[ntotal-nout];
                    for(i=0; i<=nout-1; i++)
                    {
                        mx = Math.Max(mx, network.neurons[ntotal-nout+i]);
                    }
                    net = 0;
                    for(i=0; i<=nout-1; i++)
                    {
                        network.nwbuf[i] = Math.Exp(network.neurons[ntotal-nout+i]-mx);
                        net = net+network.nwbuf[i];
                    }
                    i1_ = (0)-(ntotal-nout);
                    v = 0.0;
                    for(i_=ntotal-nout; i_<=ntotal-1;i_++)
                    {
                        v += network.derror[i_]*network.nwbuf[i_+i1_];
                    }
                    for(i=0; i<=nout-1; i++)
                    {
                        fown = network.nwbuf[i];
                        deown = network.derror[ntotal-nout+i];
                        network.nwbuf[nout+i] = (-v+deown*fown+deown*(net-fown))*fown/math.sqr(net);
                    }
                    for(i=0; i<=nout-1; i++)
                    {
                        network.derror[ntotal-nout+i] = network.nwbuf[nout+i];
                    }
                }
            }
            else
            {
                
                //
                // Un-standardisation
                //
                for(i=0; i<=nout-1; i++)
                {
                    network.derror[ntotal-nout+i] = network.derror[ntotal-nout+i]*network.columnsigmas[nin+i];
                }
            }
            
            //
            // Backpropagation
            //
            for(i=ntotal-1; i>=0; i--)
            {
                
                //
                // Extract info
                //
                offs = istart+i*nfieldwidth;
                if( network.structinfo[offs+0]>0 )
                {
                    
                    //
                    // Activation function
                    //
                    dedf = network.derror[i];
                    dfdnet = network.dfdnet[i];
                    derror[network.structinfo[offs+2]] = derror[network.structinfo[offs+2]]+dedf*dfdnet;
                }
                if( network.structinfo[offs+0]==0 )
                {
                    
                    //
                    // Adaptive summator
                    //
                    n1 = network.structinfo[offs+2];
                    n2 = n1+network.structinfo[offs+1]-1;
                    w1 = network.structinfo[offs+3];
                    w2 = w1+network.structinfo[offs+1]-1;
                    dedf = network.derror[i];
                    dfdnet = 1.0;
                    v = dedf*dfdnet;
                    i1_ = (n1) - (w1);
                    for(i_=w1; i_<=w2;i_++)
                    {
                        grad[i_] = v*neurons[i_+i1_];
                    }
                    i1_ = (w1) - (n1);
                    for(i_=n1; i_<=n2;i_++)
                    {
                        derror[i_] = derror[i_] + v*weights[i_+i1_];
                    }
                }
                if( network.structinfo[offs+0]<0 )
                {
                    bflag = false;
                    if( (network.structinfo[offs+0]==-2 | network.structinfo[offs+0]==-3) | network.structinfo[offs+0]==-4 )
                    {
                        
                        //
                        // Special neuron type, no back-propagation required
                        //
                        bflag = true;
                    }
                    ap.assert(bflag, "MLPInternalCalculateGradient: unknown neuron type!");
                }
            }
        }


        /*************************************************************************
        Internal subroutine, chunked gradient
        *************************************************************************/
        private static void mlpchunkedgradient(multilayerperceptron network,
            double[,] xy,
            int cstart,
            int csize,
            ref double e,
            ref double[] grad,
            bool naturalerrorfunc)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int kl = 0;
            int n1 = 0;
            int n2 = 0;
            int w1 = 0;
            int w2 = 0;
            int c1 = 0;
            int c2 = 0;
            int ntotal = 0;
            int nin = 0;
            int nout = 0;
            int offs = 0;
            double f = 0;
            double df = 0;
            double d2f = 0;
            double v = 0;
            double s = 0;
            double fown = 0;
            double deown = 0;
            double net = 0;
            double lnnet = 0;
            double mx = 0;
            bool bflag = new bool();
            int istart = 0;
            int ineurons = 0;
            int idfdnet = 0;
            int iderror = 0;
            int izeros = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Read network geometry, prepare data
            //
            nin = network.structinfo[1];
            nout = network.structinfo[2];
            ntotal = network.structinfo[3];
            istart = network.structinfo[5];
            c1 = cstart;
            c2 = cstart+csize-1;
            ineurons = 0;
            idfdnet = ntotal;
            iderror = 2*ntotal;
            izeros = 3*ntotal;
            for(j=0; j<=csize-1; j++)
            {
                network.chunks[izeros,j] = 0;
            }
            
            //
            // Forward pass:
            // 1. Load inputs from XY to Chunks[0:NIn-1,0:CSize-1]
            // 2. Forward pass
            //
            for(i=0; i<=nin-1; i++)
            {
                for(j=0; j<=csize-1; j++)
                {
                    if( (double)(network.columnsigmas[i])!=(double)(0) )
                    {
                        network.chunks[i,j] = (xy[c1+j,i]-network.columnmeans[i])/network.columnsigmas[i];
                    }
                    else
                    {
                        network.chunks[i,j] = xy[c1+j,i]-network.columnmeans[i];
                    }
                }
            }
            for(i=0; i<=ntotal-1; i++)
            {
                offs = istart+i*nfieldwidth;
                if( network.structinfo[offs+0]>0 )
                {
                    
                    //
                    // Activation function:
                    // * calculate F vector, F(i) = F(NET(i))
                    //
                    n1 = network.structinfo[offs+2];
                    for(i_=0; i_<=csize-1;i_++)
                    {
                        network.chunks[i,i_] = network.chunks[n1,i_];
                    }
                    for(j=0; j<=csize-1; j++)
                    {
                        mlpactivationfunction(network.chunks[i,j], network.structinfo[offs+0], ref f, ref df, ref d2f);
                        network.chunks[i,j] = f;
                        network.chunks[idfdnet+i,j] = df;
                    }
                }
                if( network.structinfo[offs+0]==0 )
                {
                    
                    //
                    // Adaptive summator:
                    // * calculate NET vector, NET(i) = SUM(W(j,i)*Neurons(j),j=N1..N2)
                    //
                    n1 = network.structinfo[offs+2];
                    n2 = n1+network.structinfo[offs+1]-1;
                    w1 = network.structinfo[offs+3];
                    w2 = w1+network.structinfo[offs+1]-1;
                    for(i_=0; i_<=csize-1;i_++)
                    {
                        network.chunks[i,i_] = network.chunks[izeros,i_];
                    }
                    for(j=n1; j<=n2; j++)
                    {
                        v = network.weights[w1+j-n1];
                        for(i_=0; i_<=csize-1;i_++)
                        {
                            network.chunks[i,i_] = network.chunks[i,i_] + v*network.chunks[j,i_];
                        }
                    }
                }
                if( network.structinfo[offs+0]<0 )
                {
                    bflag = false;
                    if( network.structinfo[offs+0]==-2 )
                    {
                        
                        //
                        // input neuron, left unchanged
                        //
                        bflag = true;
                    }
                    if( network.structinfo[offs+0]==-3 )
                    {
                        
                        //
                        // "-1" neuron
                        //
                        for(k=0; k<=csize-1; k++)
                        {
                            network.chunks[i,k] = -1;
                        }
                        bflag = true;
                    }
                    if( network.structinfo[offs+0]==-4 )
                    {
                        
                        //
                        // "0" neuron
                        //
                        for(k=0; k<=csize-1; k++)
                        {
                            network.chunks[i,k] = 0;
                        }
                        bflag = true;
                    }
                    ap.assert(bflag, "MLPChunkedGradient: internal error - unknown neuron type!");
                }
            }
            
            //
            // Post-processing, error, dError/dOut
            //
            for(i=0; i<=ntotal-1; i++)
            {
                for(i_=0; i_<=csize-1;i_++)
                {
                    network.chunks[iderror+i,i_] = network.chunks[izeros,i_];
                }
            }
            ap.assert(network.structinfo[6]==0 | network.structinfo[6]==1, "MLPChunkedGradient: unknown normalization type!");
            if( network.structinfo[6]==1 )
            {
                
                //
                // Softmax output, classification network.
                //
                // For each K = 0..CSize-1 do:
                // 1. place exp(outputs[k]) to NWBuf[0:NOut-1]
                // 2. place sum(exp(..)) to NET
                // 3. calculate dError/dOut and place it to the second block of Chunks
                //
                for(k=0; k<=csize-1; k++)
                {
                    
                    //
                    // Normalize
                    //
                    mx = network.chunks[ntotal-nout,k];
                    for(i=1; i<=nout-1; i++)
                    {
                        mx = Math.Max(mx, network.chunks[ntotal-nout+i,k]);
                    }
                    net = 0;
                    for(i=0; i<=nout-1; i++)
                    {
                        network.nwbuf[i] = Math.Exp(network.chunks[ntotal-nout+i,k]-mx);
                        net = net+network.nwbuf[i];
                    }
                    
                    //
                    // Calculate error function and dError/dOut
                    //
                    if( naturalerrorfunc )
                    {
                        
                        //
                        // Natural error func.
                        //
                        //
                        s = 1;
                        lnnet = Math.Log(net);
                        kl = (int)Math.Round(xy[cstart+k,nin]);
                        for(i=0; i<=nout-1; i++)
                        {
                            if( i==kl )
                            {
                                v = 1;
                            }
                            else
                            {
                                v = 0;
                            }
                            network.chunks[iderror+ntotal-nout+i,k] = s*network.nwbuf[i]/net-v;
                            e = e+safecrossentropy(v, network.nwbuf[i]/net);
                        }
                    }
                    else
                    {
                        
                        //
                        // Least squares error func
                        // Error, dError/dOut(normalized)
                        //
                        kl = (int)Math.Round(xy[cstart+k,nin]);
                        for(i=0; i<=nout-1; i++)
                        {
                            if( i==kl )
                            {
                                v = network.nwbuf[i]/net-1;
                            }
                            else
                            {
                                v = network.nwbuf[i]/net;
                            }
                            network.nwbuf[nout+i] = v;
                            e = e+math.sqr(v)/2;
                        }
                        
                        //
                        // From dError/dOut(normalized) to dError/dOut(non-normalized)
                        //
                        i1_ = (0)-(nout);
                        v = 0.0;
                        for(i_=nout; i_<=2*nout-1;i_++)
                        {
                            v += network.nwbuf[i_]*network.nwbuf[i_+i1_];
                        }
                        for(i=0; i<=nout-1; i++)
                        {
                            fown = network.nwbuf[i];
                            deown = network.nwbuf[nout+i];
                            network.chunks[iderror+ntotal-nout+i,k] = (-v+deown*fown+deown*(net-fown))*fown/math.sqr(net);
                        }
                    }
                }
            }
            else
            {
                
                //
                // Normal output, regression network
                //
                // For each K = 0..CSize-1 do:
                // 1. calculate dError/dOut and place it to the second block of Chunks
                //
                for(i=0; i<=nout-1; i++)
                {
                    for(j=0; j<=csize-1; j++)
                    {
                        v = network.chunks[ntotal-nout+i,j]*network.columnsigmas[nin+i]+network.columnmeans[nin+i]-xy[cstart+j,nin+i];
                        network.chunks[iderror+ntotal-nout+i,j] = v*network.columnsigmas[nin+i];
                        e = e+math.sqr(v)/2;
                    }
                }
            }
            
            //
            // Backpropagation
            //
            for(i=ntotal-1; i>=0; i--)
            {
                
                //
                // Extract info
                //
                offs = istart+i*nfieldwidth;
                if( network.structinfo[offs+0]>0 )
                {
                    
                    //
                    // Activation function
                    //
                    n1 = network.structinfo[offs+2];
                    for(k=0; k<=csize-1; k++)
                    {
                        network.chunks[iderror+i,k] = network.chunks[iderror+i,k]*network.chunks[idfdnet+i,k];
                    }
                    for(i_=0; i_<=csize-1;i_++)
                    {
                        network.chunks[iderror+n1,i_] = network.chunks[iderror+n1,i_] + network.chunks[iderror+i,i_];
                    }
                }
                if( network.structinfo[offs+0]==0 )
                {
                    
                    //
                    // "Normal" activation function
                    //
                    n1 = network.structinfo[offs+2];
                    n2 = n1+network.structinfo[offs+1]-1;
                    w1 = network.structinfo[offs+3];
                    w2 = w1+network.structinfo[offs+1]-1;
                    for(j=w1; j<=w2; j++)
                    {
                        v = 0.0;
                        for(i_=0; i_<=csize-1;i_++)
                        {
                            v += network.chunks[n1+j-w1,i_]*network.chunks[iderror+i,i_];
                        }
                        grad[j] = grad[j]+v;
                    }
                    for(j=n1; j<=n2; j++)
                    {
                        v = network.weights[w1+j-n1];
                        for(i_=0; i_<=csize-1;i_++)
                        {
                            network.chunks[iderror+j,i_] = network.chunks[iderror+j,i_] + v*network.chunks[iderror+i,i_];
                        }
                    }
                }
                if( network.structinfo[offs+0]<0 )
                {
                    bflag = false;
                    if( (network.structinfo[offs+0]==-2 | network.structinfo[offs+0]==-3) | network.structinfo[offs+0]==-4 )
                    {
                        
                        //
                        // Special neuron type, no back-propagation required
                        //
                        bflag = true;
                    }
                    ap.assert(bflag, "MLPInternalCalculateGradient: unknown neuron type!");
                }
            }
        }


        /*************************************************************************
        Returns T*Ln(T/Z), guarded against overflow/underflow.
        Internal subroutine.
        *************************************************************************/
        private static double safecrossentropy(double t,
            double z)
        {
            double result = 0;
            double r = 0;

            if( (double)(t)==(double)(0) )
            {
                result = 0;
            }
            else
            {
                if( (double)(Math.Abs(z))>(double)(1) )
                {
                    
                    //
                    // Shouldn't be the case with softmax,
                    // but we just want to be sure.
                    //
                    if( (double)(t/z)==(double)(0) )
                    {
                        r = math.minrealnumber;
                    }
                    else
                    {
                        r = t/z;
                    }
                }
                else
                {
                    
                    //
                    // Normal case
                    //
                    if( (double)(z)==(double)(0) | (double)(Math.Abs(t))>=(double)(math.maxrealnumber*Math.Abs(z)) )
                    {
                        r = math.maxrealnumber;
                    }
                    else
                    {
                        r = t/z;
                    }
                }
                result = t*Math.Log(r);
            }
            return result;
        }


    }
    public class logit
    {
        public class logitmodel
        {
            public double[] w;
            public logitmodel()
            {
                w = new double[0];
            }
        };


        public class logitmcstate
        {
            public bool brackt;
            public bool stage1;
            public int infoc;
            public double dg;
            public double dgm;
            public double dginit;
            public double dgtest;
            public double dgx;
            public double dgxm;
            public double dgy;
            public double dgym;
            public double finit;
            public double ftest1;
            public double fm;
            public double fx;
            public double fxm;
            public double fy;
            public double fym;
            public double stx;
            public double sty;
            public double stmin;
            public double stmax;
            public double width;
            public double width1;
            public double xtrapf;
        };


        /*************************************************************************
        MNLReport structure contains information about training process:
        * NGrad     -   number of gradient calculations
        * NHess     -   number of Hessian calculations
        *************************************************************************/
        public class mnlreport
        {
            public int ngrad;
            public int nhess;
        };




        public const double xtol = 100*math.machineepsilon;
        public const double ftol = 0.0001;
        public const double gtol = 0.3;
        public const int maxfev = 20;
        public const double stpmin = 1.0E-2;
        public const double stpmax = 1.0E5;
        public const int logitvnum = 6;


        /*************************************************************************
        This subroutine trains logit model.

        INPUT PARAMETERS:
            XY          -   training set, array[0..NPoints-1,0..NVars]
                            First NVars columns store values of independent
                            variables, next column stores number of class (from 0
                            to NClasses-1) which dataset element belongs to. Fractional
                            values are rounded to nearest integer.
            NPoints     -   training set size, NPoints>=1
            NVars       -   number of independent variables, NVars>=1
            NClasses    -   number of classes, NClasses>=2

        OUTPUT PARAMETERS:
            Info        -   return code:
                            * -2, if there is a point with class number
                                  outside of [0..NClasses-1].
                            * -1, if incorrect parameters was passed
                                  (NPoints<NVars+2, NVars<1, NClasses<2).
                            *  1, if task has been solved
            LM          -   model built
            Rep         -   training report

          -- ALGLIB --
             Copyright 10.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mnltrainh(double[,] xy,
            int npoints,
            int nvars,
            int nclasses,
            ref int info,
            logitmodel lm,
            mnlreport rep)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int ssize = 0;
            bool allsame = new bool();
            int offs = 0;
            double threshold = 0;
            double wminstep = 0;
            double decay = 0;
            int wdim = 0;
            int expoffs = 0;
            double v = 0;
            double s = 0;
            mlpbase.multilayerperceptron network = new mlpbase.multilayerperceptron();
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            double e = 0;
            double[] g = new double[0];
            double[,] h = new double[0,0];
            bool spd = new bool();
            double[] x = new double[0];
            double[] y = new double[0];
            double[] wbase = new double[0];
            double wstep = 0;
            double[] wdir = new double[0];
            double[] work = new double[0];
            int mcstage = 0;
            logitmcstate mcstate = new logitmcstate();
            int mcinfo = 0;
            int mcnfev = 0;
            int solverinfo = 0;
            densesolver.densesolverreport solverrep = new densesolver.densesolverreport();
            int i_ = 0;
            int i1_ = 0;

            info = 0;

            threshold = 1000*math.machineepsilon;
            wminstep = 0.001;
            decay = 0.001;
            
            //
            // Test for inputs
            //
            if( (npoints<nvars+2 | nvars<1) | nclasses<2 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=npoints-1; i++)
            {
                if( (int)Math.Round(xy[i,nvars])<0 | (int)Math.Round(xy[i,nvars])>=nclasses )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // Initialize data
            //
            rep.ngrad = 0;
            rep.nhess = 0;
            
            //
            // Allocate array
            //
            wdim = (nvars+1)*(nclasses-1);
            offs = 5;
            expoffs = offs+wdim;
            ssize = 5+(nvars+1)*(nclasses-1)+nclasses;
            lm.w = new double[ssize-1+1];
            lm.w[0] = ssize;
            lm.w[1] = logitvnum;
            lm.w[2] = nvars;
            lm.w[3] = nclasses;
            lm.w[4] = offs;
            
            //
            // Degenerate case: all outputs are equal
            //
            allsame = true;
            for(i=1; i<=npoints-1; i++)
            {
                if( (int)Math.Round(xy[i,nvars])!=(int)Math.Round(xy[i-1,nvars]) )
                {
                    allsame = false;
                }
            }
            if( allsame )
            {
                for(i=0; i<=(nvars+1)*(nclasses-1)-1; i++)
                {
                    lm.w[offs+i] = 0;
                }
                v = -(2*Math.Log(math.minrealnumber));
                k = (int)Math.Round(xy[0,nvars]);
                if( k==nclasses-1 )
                {
                    for(i=0; i<=nclasses-2; i++)
                    {
                        lm.w[offs+i*(nvars+1)+nvars] = -v;
                    }
                }
                else
                {
                    for(i=0; i<=nclasses-2; i++)
                    {
                        if( i==k )
                        {
                            lm.w[offs+i*(nvars+1)+nvars] = v;
                        }
                        else
                        {
                            lm.w[offs+i*(nvars+1)+nvars] = 0;
                        }
                    }
                }
                return;
            }
            
            //
            // General case.
            // Prepare task and network. Allocate space.
            //
            mlpbase.mlpcreatec0(nvars, nclasses, network);
            mlpbase.mlpinitpreprocessor(network, xy, npoints);
            mlpbase.mlpproperties(network, ref nin, ref nout, ref wcount);
            for(i=0; i<=wcount-1; i++)
            {
                network.weights[i] = (2*math.randomreal()-1)/nvars;
            }
            g = new double[wcount-1+1];
            h = new double[wcount-1+1, wcount-1+1];
            wbase = new double[wcount-1+1];
            wdir = new double[wcount-1+1];
            work = new double[wcount-1+1];
            
            //
            // First stage: optimize in gradient direction.
            //
            for(k=0; k<=wcount/3+10; k++)
            {
                
                //
                // Calculate gradient in starting point
                //
                mlpbase.mlpgradnbatch(network, xy, npoints, ref e, ref g);
                v = 0.0;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    v += network.weights[i_]*network.weights[i_];
                }
                e = e+0.5*decay*v;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    g[i_] = g[i_] + decay*network.weights[i_];
                }
                rep.ngrad = rep.ngrad+1;
                
                //
                // Setup optimization scheme
                //
                for(i_=0; i_<=wcount-1;i_++)
                {
                    wdir[i_] = -g[i_];
                }
                v = 0.0;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    v += wdir[i_]*wdir[i_];
                }
                wstep = Math.Sqrt(v);
                v = 1/Math.Sqrt(v);
                for(i_=0; i_<=wcount-1;i_++)
                {
                    wdir[i_] = v*wdir[i_];
                }
                mcstage = 0;
                mnlmcsrch(wcount, ref network.weights, ref e, ref g, wdir, ref wstep, ref mcinfo, ref mcnfev, ref work, mcstate, ref mcstage);
                while( mcstage!=0 )
                {
                    mlpbase.mlpgradnbatch(network, xy, npoints, ref e, ref g);
                    v = 0.0;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        v += network.weights[i_]*network.weights[i_];
                    }
                    e = e+0.5*decay*v;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        g[i_] = g[i_] + decay*network.weights[i_];
                    }
                    rep.ngrad = rep.ngrad+1;
                    mnlmcsrch(wcount, ref network.weights, ref e, ref g, wdir, ref wstep, ref mcinfo, ref mcnfev, ref work, mcstate, ref mcstage);
                }
            }
            
            //
            // Second stage: use Hessian when we are close to the minimum
            //
            while( true )
            {
                
                //
                // Calculate and update E/G/H
                //
                mlpbase.mlphessiannbatch(network, xy, npoints, ref e, ref g, ref h);
                v = 0.0;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    v += network.weights[i_]*network.weights[i_];
                }
                e = e+0.5*decay*v;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    g[i_] = g[i_] + decay*network.weights[i_];
                }
                for(k=0; k<=wcount-1; k++)
                {
                    h[k,k] = h[k,k]+decay;
                }
                rep.nhess = rep.nhess+1;
                
                //
                // Select step direction
                // NOTE: it is important to use lower-triangle Cholesky
                // factorization since it is much faster than higher-triangle version.
                //
                spd = trfac.spdmatrixcholesky(ref h, wcount, false);
                densesolver.spdmatrixcholeskysolve(h, wcount, false, g, ref solverinfo, solverrep, ref wdir);
                spd = solverinfo>0;
                if( spd )
                {
                    
                    //
                    // H is positive definite.
                    // Step in Newton direction.
                    //
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        wdir[i_] = -1*wdir[i_];
                    }
                    spd = true;
                }
                else
                {
                    
                    //
                    // H is indefinite.
                    // Step in gradient direction.
                    //
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        wdir[i_] = -g[i_];
                    }
                    spd = false;
                }
                
                //
                // Optimize in WDir direction
                //
                v = 0.0;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    v += wdir[i_]*wdir[i_];
                }
                wstep = Math.Sqrt(v);
                v = 1/Math.Sqrt(v);
                for(i_=0; i_<=wcount-1;i_++)
                {
                    wdir[i_] = v*wdir[i_];
                }
                mcstage = 0;
                mnlmcsrch(wcount, ref network.weights, ref e, ref g, wdir, ref wstep, ref mcinfo, ref mcnfev, ref work, mcstate, ref mcstage);
                while( mcstage!=0 )
                {
                    mlpbase.mlpgradnbatch(network, xy, npoints, ref e, ref g);
                    v = 0.0;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        v += network.weights[i_]*network.weights[i_];
                    }
                    e = e+0.5*decay*v;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        g[i_] = g[i_] + decay*network.weights[i_];
                    }
                    rep.ngrad = rep.ngrad+1;
                    mnlmcsrch(wcount, ref network.weights, ref e, ref g, wdir, ref wstep, ref mcinfo, ref mcnfev, ref work, mcstate, ref mcstage);
                }
                if( spd & ((mcinfo==2 | mcinfo==4) | mcinfo==6) )
                {
                    break;
                }
            }
            
            //
            // Convert from NN format to MNL format
            //
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+wcount-1;i_++)
            {
                lm.w[i_] = network.weights[i_+i1_];
            }
            for(k=0; k<=nvars-1; k++)
            {
                for(i=0; i<=nclasses-2; i++)
                {
                    s = network.columnsigmas[k];
                    if( (double)(s)==(double)(0) )
                    {
                        s = 1;
                    }
                    j = offs+(nvars+1)*i;
                    v = lm.w[j+k];
                    lm.w[j+k] = v/s;
                    lm.w[j+nvars] = lm.w[j+nvars]+v*network.columnmeans[k]/s;
                }
            }
            for(k=0; k<=nclasses-2; k++)
            {
                lm.w[offs+(nvars+1)*k+nvars] = -lm.w[offs+(nvars+1)*k+nvars];
            }
        }


        /*************************************************************************
        Procesing

        INPUT PARAMETERS:
            LM      -   logit model, passed by non-constant reference
                        (some fields of structure are used as temporaries
                        when calculating model output).
            X       -   input vector,  array[0..NVars-1].
            Y       -   (possibly) preallocated buffer; if size of Y is less than
                        NClasses, it will be reallocated.If it is large enough, it
                        is NOT reallocated, so we can save some time on reallocation.

        OUTPUT PARAMETERS:
            Y       -   result, array[0..NClasses-1]
                        Vector of posterior probabilities for classification task.

          -- ALGLIB --
             Copyright 10.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mnlprocess(logitmodel lm,
            double[] x,
            ref double[] y)
        {
            int nvars = 0;
            int nclasses = 0;
            int offs = 0;
            int i = 0;
            int i1 = 0;
            double s = 0;

            ap.assert((double)(lm.w[1])==(double)(logitvnum), "MNLProcess: unexpected model version");
            nvars = (int)Math.Round(lm.w[2]);
            nclasses = (int)Math.Round(lm.w[3]);
            offs = (int)Math.Round(lm.w[4]);
            mnliexp(ref lm.w, x);
            s = 0;
            i1 = offs+(nvars+1)*(nclasses-1);
            for(i=i1; i<=i1+nclasses-1; i++)
            {
                s = s+lm.w[i];
            }
            if( ap.len(y)<nclasses )
            {
                y = new double[nclasses];
            }
            for(i=0; i<=nclasses-1; i++)
            {
                y[i] = lm.w[i1+i]/s;
            }
        }


        /*************************************************************************
        'interactive'  variant  of  MNLProcess  for  languages  like  Python which
        support constructs like "Y = MNLProcess(LM,X)" and interactive mode of the
        interpreter

        This function allocates new array on each call,  so  it  is  significantly
        slower than its 'non-interactive' counterpart, but it is  more  convenient
        when you call it from command line.

          -- ALGLIB --
             Copyright 10.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mnlprocessi(logitmodel lm,
            double[] x,
            ref double[] y)
        {
            y = new double[0];

            mnlprocess(lm, x, ref y);
        }


        /*************************************************************************
        Unpacks coefficients of logit model. Logit model have form:

            P(class=i) = S(i) / (S(0) + S(1) + ... +S(M-1))
                  S(i) = Exp(A[i,0]*X[0] + ... + A[i,N-1]*X[N-1] + A[i,N]), when i<M-1
                S(M-1) = 1

        INPUT PARAMETERS:
            LM          -   logit model in ALGLIB format

        OUTPUT PARAMETERS:
            V           -   coefficients, array[0..NClasses-2,0..NVars]
            NVars       -   number of independent variables
            NClasses    -   number of classes

          -- ALGLIB --
             Copyright 10.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mnlunpack(logitmodel lm,
            ref double[,] a,
            ref int nvars,
            ref int nclasses)
        {
            int offs = 0;
            int i = 0;
            int i_ = 0;
            int i1_ = 0;

            a = new double[0,0];
            nvars = 0;
            nclasses = 0;

            ap.assert((double)(lm.w[1])==(double)(logitvnum), "MNLUnpack: unexpected model version");
            nvars = (int)Math.Round(lm.w[2]);
            nclasses = (int)Math.Round(lm.w[3]);
            offs = (int)Math.Round(lm.w[4]);
            a = new double[nclasses-2+1, nvars+1];
            for(i=0; i<=nclasses-2; i++)
            {
                i1_ = (offs+i*(nvars+1)) - (0);
                for(i_=0; i_<=nvars;i_++)
                {
                    a[i,i_] = lm.w[i_+i1_];
                }
            }
        }


        /*************************************************************************
        "Packs" coefficients and creates logit model in ALGLIB format (MNLUnpack
        reversed).

        INPUT PARAMETERS:
            A           -   model (see MNLUnpack)
            NVars       -   number of independent variables
            NClasses    -   number of classes

        OUTPUT PARAMETERS:
            LM          -   logit model.

          -- ALGLIB --
             Copyright 10.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mnlpack(double[,] a,
            int nvars,
            int nclasses,
            logitmodel lm)
        {
            int offs = 0;
            int i = 0;
            int wdim = 0;
            int ssize = 0;
            int i_ = 0;
            int i1_ = 0;

            wdim = (nvars+1)*(nclasses-1);
            offs = 5;
            ssize = 5+(nvars+1)*(nclasses-1)+nclasses;
            lm.w = new double[ssize-1+1];
            lm.w[0] = ssize;
            lm.w[1] = logitvnum;
            lm.w[2] = nvars;
            lm.w[3] = nclasses;
            lm.w[4] = offs;
            for(i=0; i<=nclasses-2; i++)
            {
                i1_ = (0) - (offs+i*(nvars+1));
                for(i_=offs+i*(nvars+1); i_<=offs+i*(nvars+1)+nvars;i_++)
                {
                    lm.w[i_] = a[i,i_+i1_];
                }
            }
        }


        /*************************************************************************
        Copying of LogitModel strucure

        INPUT PARAMETERS:
            LM1 -   original

        OUTPUT PARAMETERS:
            LM2 -   copy

          -- ALGLIB --
             Copyright 15.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mnlcopy(logitmodel lm1,
            logitmodel lm2)
        {
            int k = 0;
            int i_ = 0;

            k = (int)Math.Round(lm1.w[0]);
            lm2.w = new double[k-1+1];
            for(i_=0; i_<=k-1;i_++)
            {
                lm2.w[i_] = lm1.w[i_];
            }
        }


        /*************************************************************************
        Average cross-entropy (in bits per element) on the test set

        INPUT PARAMETERS:
            LM      -   logit model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            CrossEntropy/(NPoints*ln(2)).

          -- ALGLIB --
             Copyright 10.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mnlavgce(logitmodel lm,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            int nvars = 0;
            int nclasses = 0;
            int i = 0;
            double[] workx = new double[0];
            double[] worky = new double[0];
            int i_ = 0;

            ap.assert((double)(lm.w[1])==(double)(logitvnum), "MNLClsError: unexpected model version");
            nvars = (int)Math.Round(lm.w[2]);
            nclasses = (int)Math.Round(lm.w[3]);
            workx = new double[nvars-1+1];
            worky = new double[nclasses-1+1];
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                ap.assert((int)Math.Round(xy[i,nvars])>=0 & (int)Math.Round(xy[i,nvars])<nclasses, "MNLAvgCE: incorrect class number!");
                
                //
                // Process
                //
                for(i_=0; i_<=nvars-1;i_++)
                {
                    workx[i_] = xy[i,i_];
                }
                mnlprocess(lm, workx, ref worky);
                if( (double)(worky[(int)Math.Round(xy[i,nvars])])>(double)(0) )
                {
                    result = result-Math.Log(worky[(int)Math.Round(xy[i,nvars])]);
                }
                else
                {
                    result = result-Math.Log(math.minrealnumber);
                }
            }
            result = result/(npoints*Math.Log(2));
            return result;
        }


        /*************************************************************************
        Relative classification error on the test set

        INPUT PARAMETERS:
            LM      -   logit model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            percent of incorrectly classified cases.

          -- ALGLIB --
             Copyright 10.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mnlrelclserror(logitmodel lm,
            double[,] xy,
            int npoints)
        {
            double result = 0;

            result = (double)mnlclserror(lm, xy, npoints)/(double)npoints;
            return result;
        }


        /*************************************************************************
        RMS error on the test set

        INPUT PARAMETERS:
            LM      -   logit model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            root mean square error (error when estimating posterior probabilities).

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mnlrmserror(logitmodel lm,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            ap.assert((int)Math.Round(lm.w[1])==logitvnum, "MNLRMSError: Incorrect MNL version!");
            mnlallerrors(lm, xy, npoints, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = rms;
            return result;
        }


        /*************************************************************************
        Average error on the test set

        INPUT PARAMETERS:
            LM      -   logit model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            average error (error when estimating posterior probabilities).

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mnlavgerror(logitmodel lm,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            ap.assert((int)Math.Round(lm.w[1])==logitvnum, "MNLRMSError: Incorrect MNL version!");
            mnlallerrors(lm, xy, npoints, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = avg;
            return result;
        }


        /*************************************************************************
        Average relative error on the test set

        INPUT PARAMETERS:
            LM      -   logit model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            average relative error (error when estimating posterior probabilities).

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mnlavgrelerror(logitmodel lm,
            double[,] xy,
            int ssize)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            ap.assert((int)Math.Round(lm.w[1])==logitvnum, "MNLRMSError: Incorrect MNL version!");
            mnlallerrors(lm, xy, ssize, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = avgrel;
            return result;
        }


        /*************************************************************************
        Classification error on test set = MNLRelClsError*NPoints

          -- ALGLIB --
             Copyright 10.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static int mnlclserror(logitmodel lm,
            double[,] xy,
            int npoints)
        {
            int result = 0;
            int nvars = 0;
            int nclasses = 0;
            int i = 0;
            int j = 0;
            double[] workx = new double[0];
            double[] worky = new double[0];
            int nmax = 0;
            int i_ = 0;

            ap.assert((double)(lm.w[1])==(double)(logitvnum), "MNLClsError: unexpected model version");
            nvars = (int)Math.Round(lm.w[2]);
            nclasses = (int)Math.Round(lm.w[3]);
            workx = new double[nvars-1+1];
            worky = new double[nclasses-1+1];
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                
                //
                // Process
                //
                for(i_=0; i_<=nvars-1;i_++)
                {
                    workx[i_] = xy[i,i_];
                }
                mnlprocess(lm, workx, ref worky);
                
                //
                // Logit version of the answer
                //
                nmax = 0;
                for(j=0; j<=nclasses-1; j++)
                {
                    if( (double)(worky[j])>(double)(worky[nmax]) )
                    {
                        nmax = j;
                    }
                }
                
                //
                // compare
                //
                if( nmax!=(int)Math.Round(xy[i,nvars]) )
                {
                    result = result+1;
                }
            }
            return result;
        }


        /*************************************************************************
        Internal subroutine. Places exponents of the anti-overflow shifted
        internal linear outputs into the service part of the W array.
        *************************************************************************/
        private static void mnliexp(ref double[] w,
            double[] x)
        {
            int nvars = 0;
            int nclasses = 0;
            int offs = 0;
            int i = 0;
            int i1 = 0;
            double v = 0;
            double mx = 0;
            int i_ = 0;
            int i1_ = 0;

            ap.assert((double)(w[1])==(double)(logitvnum), "LOGIT: unexpected model version");
            nvars = (int)Math.Round(w[2]);
            nclasses = (int)Math.Round(w[3]);
            offs = (int)Math.Round(w[4]);
            i1 = offs+(nvars+1)*(nclasses-1);
            for(i=0; i<=nclasses-2; i++)
            {
                i1_ = (0)-(offs+i*(nvars+1));
                v = 0.0;
                for(i_=offs+i*(nvars+1); i_<=offs+i*(nvars+1)+nvars-1;i_++)
                {
                    v += w[i_]*x[i_+i1_];
                }
                w[i1+i] = v+w[offs+i*(nvars+1)+nvars];
            }
            w[i1+nclasses-1] = 0;
            mx = 0;
            for(i=i1; i<=i1+nclasses-1; i++)
            {
                mx = Math.Max(mx, w[i]);
            }
            for(i=i1; i<=i1+nclasses-1; i++)
            {
                w[i] = Math.Exp(w[i]-mx);
            }
        }


        /*************************************************************************
        Calculation of all types of errors

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        private static void mnlallerrors(logitmodel lm,
            double[,] xy,
            int npoints,
            ref double relcls,
            ref double avgce,
            ref double rms,
            ref double avg,
            ref double avgrel)
        {
            int nvars = 0;
            int nclasses = 0;
            int i = 0;
            double[] buf = new double[0];
            double[] workx = new double[0];
            double[] y = new double[0];
            double[] dy = new double[0];
            int i_ = 0;

            relcls = 0;
            avgce = 0;
            rms = 0;
            avg = 0;
            avgrel = 0;

            ap.assert((int)Math.Round(lm.w[1])==logitvnum, "MNL unit: Incorrect MNL version!");
            nvars = (int)Math.Round(lm.w[2]);
            nclasses = (int)Math.Round(lm.w[3]);
            workx = new double[nvars-1+1];
            y = new double[nclasses-1+1];
            dy = new double[0+1];
            bdss.dserrallocate(nclasses, ref buf);
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    workx[i_] = xy[i,i_];
                }
                mnlprocess(lm, workx, ref y);
                dy[0] = xy[i,nvars];
                bdss.dserraccumulate(ref buf, y, dy);
            }
            bdss.dserrfinish(ref buf);
            relcls = buf[0];
            avgce = buf[1];
            rms = buf[2];
            avg = buf[3];
            avgrel = buf[4];
        }


        /*************************************************************************
        THE  PURPOSE  OF  MCSRCH  IS  TO  FIND A STEP WHICH SATISFIES A SUFFICIENT
        DECREASE CONDITION AND A CURVATURE CONDITION.

        AT EACH STAGE THE SUBROUTINE  UPDATES  AN  INTERVAL  OF  UNCERTAINTY  WITH
        ENDPOINTS  STX  AND  STY.  THE INTERVAL OF UNCERTAINTY IS INITIALLY CHOSEN
        SO THAT IT CONTAINS A MINIMIZER OF THE MODIFIED FUNCTION

            F(X+STP*S) - F(X) - FTOL*STP*(GRADF(X)'S).

        IF  A STEP  IS OBTAINED FOR  WHICH THE MODIFIED FUNCTION HAS A NONPOSITIVE
        FUNCTION  VALUE  AND  NONNEGATIVE  DERIVATIVE,   THEN   THE   INTERVAL  OF
        UNCERTAINTY IS CHOSEN SO THAT IT CONTAINS A MINIMIZER OF F(X+STP*S).

        THE  ALGORITHM  IS  DESIGNED TO FIND A STEP WHICH SATISFIES THE SUFFICIENT
        DECREASE CONDITION

            F(X+STP*S) .LE. F(X) + FTOL*STP*(GRADF(X)'S),

        AND THE CURVATURE CONDITION

            ABS(GRADF(X+STP*S)'S)) .LE. GTOL*ABS(GRADF(X)'S).

        IF  FTOL  IS  LESS  THAN GTOL AND IF, FOR EXAMPLE, THE FUNCTION IS BOUNDED
        BELOW,  THEN  THERE  IS  ALWAYS  A  STEP  WHICH SATISFIES BOTH CONDITIONS.
        IF  NO  STEP  CAN BE FOUND  WHICH  SATISFIES  BOTH  CONDITIONS,  THEN  THE
        ALGORITHM  USUALLY STOPS  WHEN  ROUNDING ERRORS  PREVENT FURTHER PROGRESS.
        IN THIS CASE STP ONLY SATISFIES THE SUFFICIENT DECREASE CONDITION.

        PARAMETERS DESCRIPRION

        N IS A POSITIVE INTEGER INPUT VARIABLE SET TO THE NUMBER OF VARIABLES.

        X IS  AN  ARRAY  OF  LENGTH N. ON INPUT IT MUST CONTAIN THE BASE POINT FOR
        THE LINE SEARCH. ON OUTPUT IT CONTAINS X+STP*S.

        F IS  A  VARIABLE. ON INPUT IT MUST CONTAIN THE VALUE OF F AT X. ON OUTPUT
        IT CONTAINS THE VALUE OF F AT X + STP*S.

        G IS AN ARRAY OF LENGTH N. ON INPUT IT MUST CONTAIN THE GRADIENT OF F AT X.
        ON OUTPUT IT CONTAINS THE GRADIENT OF F AT X + STP*S.

        S IS AN INPUT ARRAY OF LENGTH N WHICH SPECIFIES THE SEARCH DIRECTION.

        STP  IS  A NONNEGATIVE VARIABLE. ON INPUT STP CONTAINS AN INITIAL ESTIMATE
        OF A SATISFACTORY STEP. ON OUTPUT STP CONTAINS THE FINAL ESTIMATE.

        FTOL AND GTOL ARE NONNEGATIVE INPUT VARIABLES. TERMINATION OCCURS WHEN THE
        SUFFICIENT DECREASE CONDITION AND THE DIRECTIONAL DERIVATIVE CONDITION ARE
        SATISFIED.

        XTOL IS A NONNEGATIVE INPUT VARIABLE. TERMINATION OCCURS WHEN THE RELATIVE
        WIDTH OF THE INTERVAL OF UNCERTAINTY IS AT MOST XTOL.

        STPMIN AND STPMAX ARE NONNEGATIVE INPUT VARIABLES WHICH SPECIFY LOWER  AND
        UPPER BOUNDS FOR THE STEP.

        MAXFEV IS A POSITIVE INTEGER INPUT VARIABLE. TERMINATION OCCURS WHEN THE
        NUMBER OF CALLS TO FCN IS AT LEAST MAXFEV BY THE END OF AN ITERATION.

        INFO IS AN INTEGER OUTPUT VARIABLE SET AS FOLLOWS:
            INFO = 0  IMPROPER INPUT PARAMETERS.

            INFO = 1  THE SUFFICIENT DECREASE CONDITION AND THE
                      DIRECTIONAL DERIVATIVE CONDITION HOLD.

            INFO = 2  RELATIVE WIDTH OF THE INTERVAL OF UNCERTAINTY
                      IS AT MOST XTOL.

            INFO = 3  NUMBER OF CALLS TO FCN HAS REACHED MAXFEV.

            INFO = 4  THE STEP IS AT THE LOWER BOUND STPMIN.

            INFO = 5  THE STEP IS AT THE UPPER BOUND STPMAX.

            INFO = 6  ROUNDING ERRORS PREVENT FURTHER PROGRESS.
                      THERE MAY NOT BE A STEP WHICH SATISFIES THE
                      SUFFICIENT DECREASE AND CURVATURE CONDITIONS.
                      TOLERANCES MAY BE TOO SMALL.

        NFEV IS AN INTEGER OUTPUT VARIABLE SET TO THE NUMBER OF CALLS TO FCN.

        WA IS A WORK ARRAY OF LENGTH N.

        ARGONNE NATIONAL LABORATORY. MINPACK PROJECT. JUNE 1983
        JORGE J. MORE', DAVID J. THUENTE
        *************************************************************************/
        private static void mnlmcsrch(int n,
            ref double[] x,
            ref double f,
            ref double[] g,
            double[] s,
            ref double stp,
            ref int info,
            ref int nfev,
            ref double[] wa,
            logitmcstate state,
            ref int stage)
        {
            double v = 0;
            double p5 = 0;
            double p66 = 0;
            double zero = 0;
            int i_ = 0;

            
            //
            // init
            //
            p5 = 0.5;
            p66 = 0.66;
            state.xtrapf = 4.0;
            zero = 0;
            
            //
            // Main cycle
            //
            while( true )
            {
                if( stage==0 )
                {
                    
                    //
                    // NEXT
                    //
                    stage = 2;
                    continue;
                }
                if( stage==2 )
                {
                    state.infoc = 1;
                    info = 0;
                    
                    //
                    //     CHECK THE INPUT PARAMETERS FOR ERRORS.
                    //
                    if( ((((((n<=0 | (double)(stp)<=(double)(0)) | (double)(ftol)<(double)(0)) | (double)(gtol)<(double)(zero)) | (double)(xtol)<(double)(zero)) | (double)(stpmin)<(double)(zero)) | (double)(stpmax)<(double)(stpmin)) | maxfev<=0 )
                    {
                        stage = 0;
                        return;
                    }
                    
                    //
                    //     COMPUTE THE INITIAL GRADIENT IN THE SEARCH DIRECTION
                    //     AND CHECK THAT S IS A DESCENT DIRECTION.
                    //
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += g[i_]*s[i_];
                    }
                    state.dginit = v;
                    if( (double)(state.dginit)>=(double)(0) )
                    {
                        stage = 0;
                        return;
                    }
                    
                    //
                    //     INITIALIZE LOCAL VARIABLES.
                    //
                    state.brackt = false;
                    state.stage1 = true;
                    nfev = 0;
                    state.finit = f;
                    state.dgtest = ftol*state.dginit;
                    state.width = stpmax-stpmin;
                    state.width1 = state.width/p5;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        wa[i_] = x[i_];
                    }
                    
                    //
                    //     THE VARIABLES STX, FX, DGX CONTAIN THE VALUES OF THE STEP,
                    //     FUNCTION, AND DIRECTIONAL DERIVATIVE AT THE BEST STEP.
                    //     THE VARIABLES STY, FY, DGY CONTAIN THE VALUE OF THE STEP,
                    //     FUNCTION, AND DERIVATIVE AT THE OTHER ENDPOINT OF
                    //     THE INTERVAL OF UNCERTAINTY.
                    //     THE VARIABLES STP, F, DG CONTAIN THE VALUES OF THE STEP,
                    //     FUNCTION, AND DERIVATIVE AT THE CURRENT STEP.
                    //
                    state.stx = 0;
                    state.fx = state.finit;
                    state.dgx = state.dginit;
                    state.sty = 0;
                    state.fy = state.finit;
                    state.dgy = state.dginit;
                    
                    //
                    // NEXT
                    //
                    stage = 3;
                    continue;
                }
                if( stage==3 )
                {
                    
                    //
                    //     START OF ITERATION.
                    //
                    //     SET THE MINIMUM AND MAXIMUM STEPS TO CORRESPOND
                    //     TO THE PRESENT INTERVAL OF UNCERTAINTY.
                    //
                    if( state.brackt )
                    {
                        if( (double)(state.stx)<(double)(state.sty) )
                        {
                            state.stmin = state.stx;
                            state.stmax = state.sty;
                        }
                        else
                        {
                            state.stmin = state.sty;
                            state.stmax = state.stx;
                        }
                    }
                    else
                    {
                        state.stmin = state.stx;
                        state.stmax = stp+state.xtrapf*(stp-state.stx);
                    }
                    
                    //
                    //        FORCE THE STEP TO BE WITHIN THE BOUNDS STPMAX AND STPMIN.
                    //
                    if( (double)(stp)>(double)(stpmax) )
                    {
                        stp = stpmax;
                    }
                    if( (double)(stp)<(double)(stpmin) )
                    {
                        stp = stpmin;
                    }
                    
                    //
                    //        IF AN UNUSUAL TERMINATION IS TO OCCUR THEN LET
                    //        STP BE THE LOWEST POINT OBTAINED SO FAR.
                    //
                    if( (((state.brackt & ((double)(stp)<=(double)(state.stmin) | (double)(stp)>=(double)(state.stmax))) | nfev>=maxfev-1) | state.infoc==0) | (state.brackt & (double)(state.stmax-state.stmin)<=(double)(xtol*state.stmax)) )
                    {
                        stp = state.stx;
                    }
                    
                    //
                    //        EVALUATE THE FUNCTION AND GRADIENT AT STP
                    //        AND COMPUTE THE DIRECTIONAL DERIVATIVE.
                    //
                    for(i_=0; i_<=n-1;i_++)
                    {
                        x[i_] = wa[i_];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        x[i_] = x[i_] + stp*s[i_];
                    }
                    
                    //
                    // NEXT
                    //
                    stage = 4;
                    return;
                }
                if( stage==4 )
                {
                    info = 0;
                    nfev = nfev+1;
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += g[i_]*s[i_];
                    }
                    state.dg = v;
                    state.ftest1 = state.finit+stp*state.dgtest;
                    
                    //
                    //        TEST FOR CONVERGENCE.
                    //
                    if( (state.brackt & ((double)(stp)<=(double)(state.stmin) | (double)(stp)>=(double)(state.stmax))) | state.infoc==0 )
                    {
                        info = 6;
                    }
                    if( ((double)(stp)==(double)(stpmax) & (double)(f)<=(double)(state.ftest1)) & (double)(state.dg)<=(double)(state.dgtest) )
                    {
                        info = 5;
                    }
                    if( (double)(stp)==(double)(stpmin) & ((double)(f)>(double)(state.ftest1) | (double)(state.dg)>=(double)(state.dgtest)) )
                    {
                        info = 4;
                    }
                    if( nfev>=maxfev )
                    {
                        info = 3;
                    }
                    if( state.brackt & (double)(state.stmax-state.stmin)<=(double)(xtol*state.stmax) )
                    {
                        info = 2;
                    }
                    if( (double)(f)<=(double)(state.ftest1) & (double)(Math.Abs(state.dg))<=(double)(-(gtol*state.dginit)) )
                    {
                        info = 1;
                    }
                    
                    //
                    //        CHECK FOR TERMINATION.
                    //
                    if( info!=0 )
                    {
                        stage = 0;
                        return;
                    }
                    
                    //
                    //        IN THE FIRST STAGE WE SEEK A STEP FOR WHICH THE MODIFIED
                    //        FUNCTION HAS A NONPOSITIVE VALUE AND NONNEGATIVE DERIVATIVE.
                    //
                    if( (state.stage1 & (double)(f)<=(double)(state.ftest1)) & (double)(state.dg)>=(double)(Math.Min(ftol, gtol)*state.dginit) )
                    {
                        state.stage1 = false;
                    }
                    
                    //
                    //        A MODIFIED FUNCTION IS USED TO PREDICT THE STEP ONLY IF
                    //        WE HAVE NOT OBTAINED A STEP FOR WHICH THE MODIFIED
                    //        FUNCTION HAS A NONPOSITIVE FUNCTION VALUE AND NONNEGATIVE
                    //        DERIVATIVE, AND IF A LOWER FUNCTION VALUE HAS BEEN
                    //        OBTAINED BUT THE DECREASE IS NOT SUFFICIENT.
                    //
                    if( (state.stage1 & (double)(f)<=(double)(state.fx)) & (double)(f)>(double)(state.ftest1) )
                    {
                        
                        //
                        //           DEFINE THE MODIFIED FUNCTION AND DERIVATIVE VALUES.
                        //
                        state.fm = f-stp*state.dgtest;
                        state.fxm = state.fx-state.stx*state.dgtest;
                        state.fym = state.fy-state.sty*state.dgtest;
                        state.dgm = state.dg-state.dgtest;
                        state.dgxm = state.dgx-state.dgtest;
                        state.dgym = state.dgy-state.dgtest;
                        
                        //
                        //           CALL CSTEP TO UPDATE THE INTERVAL OF UNCERTAINTY
                        //           AND TO COMPUTE THE NEW STEP.
                        //
                        mnlmcstep(ref state.stx, ref state.fxm, ref state.dgxm, ref state.sty, ref state.fym, ref state.dgym, ref stp, state.fm, state.dgm, ref state.brackt, state.stmin, state.stmax, ref state.infoc);
                        
                        //
                        //           RESET THE FUNCTION AND GRADIENT VALUES FOR F.
                        //
                        state.fx = state.fxm+state.stx*state.dgtest;
                        state.fy = state.fym+state.sty*state.dgtest;
                        state.dgx = state.dgxm+state.dgtest;
                        state.dgy = state.dgym+state.dgtest;
                    }
                    else
                    {
                        
                        //
                        //           CALL MCSTEP TO UPDATE THE INTERVAL OF UNCERTAINTY
                        //           AND TO COMPUTE THE NEW STEP.
                        //
                        mnlmcstep(ref state.stx, ref state.fx, ref state.dgx, ref state.sty, ref state.fy, ref state.dgy, ref stp, f, state.dg, ref state.brackt, state.stmin, state.stmax, ref state.infoc);
                    }
                    
                    //
                    //        FORCE A SUFFICIENT DECREASE IN THE SIZE OF THE
                    //        INTERVAL OF UNCERTAINTY.
                    //
                    if( state.brackt )
                    {
                        if( (double)(Math.Abs(state.sty-state.stx))>=(double)(p66*state.width1) )
                        {
                            stp = state.stx+p5*(state.sty-state.stx);
                        }
                        state.width1 = state.width;
                        state.width = Math.Abs(state.sty-state.stx);
                    }
                    
                    //
                    //  NEXT.
                    //
                    stage = 3;
                    continue;
                }
            }
        }


        private static void mnlmcstep(ref double stx,
            ref double fx,
            ref double dx,
            ref double sty,
            ref double fy,
            ref double dy,
            ref double stp,
            double fp,
            double dp,
            ref bool brackt,
            double stmin,
            double stmax,
            ref int info)
        {
            bool bound = new bool();
            double gamma = 0;
            double p = 0;
            double q = 0;
            double r = 0;
            double s = 0;
            double sgnd = 0;
            double stpc = 0;
            double stpf = 0;
            double stpq = 0;
            double theta = 0;

            info = 0;
            
            //
            //     CHECK THE INPUT PARAMETERS FOR ERRORS.
            //
            if( ((brackt & ((double)(stp)<=(double)(Math.Min(stx, sty)) | (double)(stp)>=(double)(Math.Max(stx, sty)))) | (double)(dx*(stp-stx))>=(double)(0)) | (double)(stmax)<(double)(stmin) )
            {
                return;
            }
            
            //
            //     DETERMINE IF THE DERIVATIVES HAVE OPPOSITE SIGN.
            //
            sgnd = dp*(dx/Math.Abs(dx));
            
            //
            //     FIRST CASE. A HIGHER FUNCTION VALUE.
            //     THE MINIMUM IS BRACKETED. IF THE CUBIC STEP IS CLOSER
            //     TO STX THAN THE QUADRATIC STEP, THE CUBIC STEP IS TAKEN,
            //     ELSE THE AVERAGE OF THE CUBIC AND QUADRATIC STEPS IS TAKEN.
            //
            if( (double)(fp)>(double)(fx) )
            {
                info = 1;
                bound = true;
                theta = 3*(fx-fp)/(stp-stx)+dx+dp;
                s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                gamma = s*Math.Sqrt(math.sqr(theta/s)-dx/s*(dp/s));
                if( (double)(stp)<(double)(stx) )
                {
                    gamma = -gamma;
                }
                p = gamma-dx+theta;
                q = gamma-dx+gamma+dp;
                r = p/q;
                stpc = stx+r*(stp-stx);
                stpq = stx+dx/((fx-fp)/(stp-stx)+dx)/2*(stp-stx);
                if( (double)(Math.Abs(stpc-stx))<(double)(Math.Abs(stpq-stx)) )
                {
                    stpf = stpc;
                }
                else
                {
                    stpf = stpc+(stpq-stpc)/2;
                }
                brackt = true;
            }
            else
            {
                if( (double)(sgnd)<(double)(0) )
                {
                    
                    //
                    //     SECOND CASE. A LOWER FUNCTION VALUE AND DERIVATIVES OF
                    //     OPPOSITE SIGN. THE MINIMUM IS BRACKETED. IF THE CUBIC
                    //     STEP IS CLOSER TO STX THAN THE QUADRATIC (SECANT) STEP,
                    //     THE CUBIC STEP IS TAKEN, ELSE THE QUADRATIC STEP IS TAKEN.
                    //
                    info = 2;
                    bound = false;
                    theta = 3*(fx-fp)/(stp-stx)+dx+dp;
                    s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                    gamma = s*Math.Sqrt(math.sqr(theta/s)-dx/s*(dp/s));
                    if( (double)(stp)>(double)(stx) )
                    {
                        gamma = -gamma;
                    }
                    p = gamma-dp+theta;
                    q = gamma-dp+gamma+dx;
                    r = p/q;
                    stpc = stp+r*(stx-stp);
                    stpq = stp+dp/(dp-dx)*(stx-stp);
                    if( (double)(Math.Abs(stpc-stp))>(double)(Math.Abs(stpq-stp)) )
                    {
                        stpf = stpc;
                    }
                    else
                    {
                        stpf = stpq;
                    }
                    brackt = true;
                }
                else
                {
                    if( (double)(Math.Abs(dp))<(double)(Math.Abs(dx)) )
                    {
                        
                        //
                        //     THIRD CASE. A LOWER FUNCTION VALUE, DERIVATIVES OF THE
                        //     SAME SIGN, AND THE MAGNITUDE OF THE DERIVATIVE DECREASES.
                        //     THE CUBIC STEP IS ONLY USED IF THE CUBIC TENDS TO INFINITY
                        //     IN THE DIRECTION OF THE STEP OR IF THE MINIMUM OF THE CUBIC
                        //     IS BEYOND STP. OTHERWISE THE CUBIC STEP IS DEFINED TO BE
                        //     EITHER STPMIN OR STPMAX. THE QUADRATIC (SECANT) STEP IS ALSO
                        //     COMPUTED AND IF THE MINIMUM IS BRACKETED THEN THE THE STEP
                        //     CLOSEST TO STX IS TAKEN, ELSE THE STEP FARTHEST AWAY IS TAKEN.
                        //
                        info = 3;
                        bound = true;
                        theta = 3*(fx-fp)/(stp-stx)+dx+dp;
                        s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                        
                        //
                        //        THE CASE GAMMA = 0 ONLY ARISES IF THE CUBIC DOES NOT TEND
                        //        TO INFINITY IN THE DIRECTION OF THE STEP.
                        //
                        gamma = s*Math.Sqrt(Math.Max(0, math.sqr(theta/s)-dx/s*(dp/s)));
                        if( (double)(stp)>(double)(stx) )
                        {
                            gamma = -gamma;
                        }
                        p = gamma-dp+theta;
                        q = gamma+(dx-dp)+gamma;
                        r = p/q;
                        if( (double)(r)<(double)(0) & (double)(gamma)!=(double)(0) )
                        {
                            stpc = stp+r*(stx-stp);
                        }
                        else
                        {
                            if( (double)(stp)>(double)(stx) )
                            {
                                stpc = stmax;
                            }
                            else
                            {
                                stpc = stmin;
                            }
                        }
                        stpq = stp+dp/(dp-dx)*(stx-stp);
                        if( brackt )
                        {
                            if( (double)(Math.Abs(stp-stpc))<(double)(Math.Abs(stp-stpq)) )
                            {
                                stpf = stpc;
                            }
                            else
                            {
                                stpf = stpq;
                            }
                        }
                        else
                        {
                            if( (double)(Math.Abs(stp-stpc))>(double)(Math.Abs(stp-stpq)) )
                            {
                                stpf = stpc;
                            }
                            else
                            {
                                stpf = stpq;
                            }
                        }
                    }
                    else
                    {
                        
                        //
                        //     FOURTH CASE. A LOWER FUNCTION VALUE, DERIVATIVES OF THE
                        //     SAME SIGN, AND THE MAGNITUDE OF THE DERIVATIVE DOES
                        //     NOT DECREASE. IF THE MINIMUM IS NOT BRACKETED, THE STEP
                        //     IS EITHER STPMIN OR STPMAX, ELSE THE CUBIC STEP IS TAKEN.
                        //
                        info = 4;
                        bound = false;
                        if( brackt )
                        {
                            theta = 3*(fp-fy)/(sty-stp)+dy+dp;
                            s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dy), Math.Abs(dp)));
                            gamma = s*Math.Sqrt(math.sqr(theta/s)-dy/s*(dp/s));
                            if( (double)(stp)>(double)(sty) )
                            {
                                gamma = -gamma;
                            }
                            p = gamma-dp+theta;
                            q = gamma-dp+gamma+dy;
                            r = p/q;
                            stpc = stp+r*(sty-stp);
                            stpf = stpc;
                        }
                        else
                        {
                            if( (double)(stp)>(double)(stx) )
                            {
                                stpf = stmax;
                            }
                            else
                            {
                                stpf = stmin;
                            }
                        }
                    }
                }
            }
            
            //
            //     UPDATE THE INTERVAL OF UNCERTAINTY. THIS UPDATE DOES NOT
            //     DEPEND ON THE NEW STEP OR THE CASE ANALYSIS ABOVE.
            //
            if( (double)(fp)>(double)(fx) )
            {
                sty = stp;
                fy = fp;
                dy = dp;
            }
            else
            {
                if( (double)(sgnd)<(double)(0.0) )
                {
                    sty = stx;
                    fy = fx;
                    dy = dx;
                }
                stx = stp;
                fx = fp;
                dx = dp;
            }
            
            //
            //     COMPUTE THE NEW STEP AND SAFEGUARD IT.
            //
            stpf = Math.Min(stmax, stpf);
            stpf = Math.Max(stmin, stpf);
            stp = stpf;
            if( brackt & bound )
            {
                if( (double)(sty)>(double)(stx) )
                {
                    stp = Math.Min(stx+0.66*(sty-stx), stp);
                }
                else
                {
                    stp = Math.Max(stx+0.66*(sty-stx), stp);
                }
            }
        }


    }
    public class mlptrain
    {
        /*************************************************************************
        Training report:
            * NGrad     - number of gradient calculations
            * NHess     - number of Hessian calculations
            * NCholesky - number of Cholesky decompositions
        *************************************************************************/
        public class mlpreport
        {
            public int ngrad;
            public int nhess;
            public int ncholesky;
        };


        /*************************************************************************
        Cross-validation estimates of generalization error
        *************************************************************************/
        public class mlpcvreport
        {
            public double relclserror;
            public double avgce;
            public double rmserror;
            public double avgerror;
            public double avgrelerror;
        };




        public const double mindecay = 0.001;


        /*************************************************************************
        Neural network training  using  modified  Levenberg-Marquardt  with  exact
        Hessian calculation and regularization. Subroutine trains  neural  network
        with restarts from random positions. Algorithm is well  suited  for  small
        and medium scale problems (hundreds of weights).

        INPUT PARAMETERS:
            Network     -   neural network with initialized geometry
            XY          -   training set
            NPoints     -   training set size
            Decay       -   weight decay constant, >=0.001
                            Decay term 'Decay*||Weights||^2' is added to error
                            function.
                            If you don't know what Decay to choose, use 0.001.
            Restarts    -   number of restarts from random position, >0.
                            If you don't know what Restarts to choose, use 2.

        OUTPUT PARAMETERS:
            Network     -   trained neural network.
            Info        -   return code:
                            * -9, if internal matrix inverse subroutine failed
                            * -2, if there is a point with class number
                                  outside of [0..NOut-1].
                            * -1, if wrong parameters specified
                                  (NPoints<0, Restarts<1).
                            *  2, if task has been solved.
            Rep         -   training report

          -- ALGLIB --
             Copyright 10.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlptrainlm(mlpbase.multilayerperceptron network,
            double[,] xy,
            int npoints,
            double decay,
            int restarts,
            ref int info,
            mlpreport rep)
        {
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            double lmftol = 0;
            double lmsteptol = 0;
            int i = 0;
            int k = 0;
            double v = 0;
            double e = 0;
            double enew = 0;
            double xnorm2 = 0;
            double stepnorm = 0;
            double[] g = new double[0];
            double[] d = new double[0];
            double[,] h = new double[0,0];
            double[,] hmod = new double[0,0];
            double[,] z = new double[0,0];
            bool spd = new bool();
            double nu = 0;
            double lambdav = 0;
            double lambdaup = 0;
            double lambdadown = 0;
            minlbfgs.minlbfgsreport internalrep = new minlbfgs.minlbfgsreport();
            minlbfgs.minlbfgsstate state = new minlbfgs.minlbfgsstate();
            double[] x = new double[0];
            double[] y = new double[0];
            double[] wbase = new double[0];
            double[] wdir = new double[0];
            double[] wt = new double[0];
            double[] wx = new double[0];
            int pass = 0;
            double[] wbest = new double[0];
            double ebest = 0;
            int invinfo = 0;
            matinv.matinvreport invrep = new matinv.matinvreport();
            int solverinfo = 0;
            densesolver.densesolverreport solverrep = new densesolver.densesolverreport();
            int i_ = 0;

            info = 0;

            mlpbase.mlpproperties(network, ref nin, ref nout, ref wcount);
            lambdaup = 10;
            lambdadown = 0.3;
            lmftol = 0.001;
            lmsteptol = 0.001;
            
            //
            // Test for inputs
            //
            if( npoints<=0 | restarts<1 )
            {
                info = -1;
                return;
            }
            if( mlpbase.mlpissoftmax(network) )
            {
                for(i=0; i<=npoints-1; i++)
                {
                    if( (int)Math.Round(xy[i,nin])<0 | (int)Math.Round(xy[i,nin])>=nout )
                    {
                        info = -2;
                        return;
                    }
                }
            }
            decay = Math.Max(decay, mindecay);
            info = 2;
            
            //
            // Initialize data
            //
            rep.ngrad = 0;
            rep.nhess = 0;
            rep.ncholesky = 0;
            
            //
            // General case.
            // Prepare task and network. Allocate space.
            //
            mlpbase.mlpinitpreprocessor(network, xy, npoints);
            g = new double[wcount-1+1];
            h = new double[wcount-1+1, wcount-1+1];
            hmod = new double[wcount-1+1, wcount-1+1];
            wbase = new double[wcount-1+1];
            wdir = new double[wcount-1+1];
            wbest = new double[wcount-1+1];
            wt = new double[wcount-1+1];
            wx = new double[wcount-1+1];
            ebest = math.maxrealnumber;
            
            //
            // Multiple passes
            //
            for(pass=1; pass<=restarts; pass++)
            {
                
                //
                // Initialize weights
                //
                mlpbase.mlprandomize(network);
                
                //
                // First stage of the hybrid algorithm: LBFGS
                //
                for(i_=0; i_<=wcount-1;i_++)
                {
                    wbase[i_] = network.weights[i_];
                }
                minlbfgs.minlbfgscreate(wcount, Math.Min(wcount, 5), wbase, state);
                minlbfgs.minlbfgssetcond(state, 0, 0, 0, Math.Max(25, wcount));
                while( minlbfgs.minlbfgsiteration(state) )
                {
                    
                    //
                    // gradient
                    //
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        network.weights[i_] = state.x[i_];
                    }
                    mlpbase.mlpgradbatch(network, xy, npoints, ref state.f, ref state.g);
                    
                    //
                    // weight decay
                    //
                    v = 0.0;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        v += network.weights[i_]*network.weights[i_];
                    }
                    state.f = state.f+0.5*decay*v;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        state.g[i_] = state.g[i_] + decay*network.weights[i_];
                    }
                    
                    //
                    // next iteration
                    //
                    rep.ngrad = rep.ngrad+1;
                }
                minlbfgs.minlbfgsresults(state, ref wbase, internalrep);
                for(i_=0; i_<=wcount-1;i_++)
                {
                    network.weights[i_] = wbase[i_];
                }
                
                //
                // Second stage of the hybrid algorithm: LM
                //
                // Initialize H with identity matrix,
                // G with gradient,
                // E with regularized error.
                //
                mlpbase.mlphessianbatch(network, xy, npoints, ref e, ref g, ref h);
                v = 0.0;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    v += network.weights[i_]*network.weights[i_];
                }
                e = e+0.5*decay*v;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    g[i_] = g[i_] + decay*network.weights[i_];
                }
                for(k=0; k<=wcount-1; k++)
                {
                    h[k,k] = h[k,k]+decay;
                }
                rep.nhess = rep.nhess+1;
                lambdav = 0.001;
                nu = 2;
                while( true )
                {
                    
                    //
                    // 1. HMod = H+lambda*I
                    // 2. Try to solve (H+Lambda*I)*dx = -g.
                    //    Increase lambda if left part is not positive definite.
                    //
                    for(i=0; i<=wcount-1; i++)
                    {
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            hmod[i,i_] = h[i,i_];
                        }
                        hmod[i,i] = hmod[i,i]+lambdav;
                    }
                    spd = trfac.spdmatrixcholesky(ref hmod, wcount, true);
                    rep.ncholesky = rep.ncholesky+1;
                    if( !spd )
                    {
                        lambdav = lambdav*lambdaup*nu;
                        nu = nu*2;
                        continue;
                    }
                    densesolver.spdmatrixcholeskysolve(hmod, wcount, true, g, ref solverinfo, solverrep, ref wdir);
                    if( solverinfo<0 )
                    {
                        lambdav = lambdav*lambdaup*nu;
                        nu = nu*2;
                        continue;
                    }
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        wdir[i_] = -1*wdir[i_];
                    }
                    
                    //
                    // Lambda found.
                    // 1. Save old w in WBase
                    // 1. Test some stopping criterions
                    // 2. If error(w+wdir)>error(w), increase lambda
                    //
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        network.weights[i_] = network.weights[i_] + wdir[i_];
                    }
                    xnorm2 = 0.0;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        xnorm2 += network.weights[i_]*network.weights[i_];
                    }
                    stepnorm = 0.0;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        stepnorm += wdir[i_]*wdir[i_];
                    }
                    stepnorm = Math.Sqrt(stepnorm);
                    enew = mlpbase.mlperror(network, xy, npoints)+0.5*decay*xnorm2;
                    if( (double)(stepnorm)<(double)(lmsteptol*(1+Math.Sqrt(xnorm2))) )
                    {
                        break;
                    }
                    if( (double)(enew)>(double)(e) )
                    {
                        lambdav = lambdav*lambdaup*nu;
                        nu = nu*2;
                        continue;
                    }
                    
                    //
                    // Optimize using inv(cholesky(H)) as preconditioner
                    //
                    matinv.rmatrixtrinverse(ref hmod, wcount, true, false, ref invinfo, invrep);
                    if( invinfo<=0 )
                    {
                        
                        //
                        // if matrix can't be inverted then exit with errors
                        // TODO: make WCount steps in direction suggested by HMod
                        //
                        info = -9;
                        return;
                    }
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        wbase[i_] = network.weights[i_];
                    }
                    for(i=0; i<=wcount-1; i++)
                    {
                        wt[i] = 0;
                    }
                    minlbfgs.minlbfgscreatex(wcount, wcount, wt, 1, state);
                    minlbfgs.minlbfgssetcond(state, 0, 0, 0, 5);
                    while( minlbfgs.minlbfgsiteration(state) )
                    {
                        
                        //
                        // gradient
                        //
                        for(i=0; i<=wcount-1; i++)
                        {
                            v = 0.0;
                            for(i_=i; i_<=wcount-1;i_++)
                            {
                                v += state.x[i_]*hmod[i,i_];
                            }
                            network.weights[i] = wbase[i]+v;
                        }
                        mlpbase.mlpgradbatch(network, xy, npoints, ref state.f, ref g);
                        for(i=0; i<=wcount-1; i++)
                        {
                            state.g[i] = 0;
                        }
                        for(i=0; i<=wcount-1; i++)
                        {
                            v = g[i];
                            for(i_=i; i_<=wcount-1;i_++)
                            {
                                state.g[i_] = state.g[i_] + v*hmod[i,i_];
                            }
                        }
                        
                        //
                        // weight decay
                        // grad(x'*x) = A'*(x0+A*t)
                        //
                        v = 0.0;
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            v += network.weights[i_]*network.weights[i_];
                        }
                        state.f = state.f+0.5*decay*v;
                        for(i=0; i<=wcount-1; i++)
                        {
                            v = decay*network.weights[i];
                            for(i_=i; i_<=wcount-1;i_++)
                            {
                                state.g[i_] = state.g[i_] + v*hmod[i,i_];
                            }
                        }
                        
                        //
                        // next iteration
                        //
                        rep.ngrad = rep.ngrad+1;
                    }
                    minlbfgs.minlbfgsresults(state, ref wt, internalrep);
                    
                    //
                    // Accept new position.
                    // Calculate Hessian
                    //
                    for(i=0; i<=wcount-1; i++)
                    {
                        v = 0.0;
                        for(i_=i; i_<=wcount-1;i_++)
                        {
                            v += wt[i_]*hmod[i,i_];
                        }
                        network.weights[i] = wbase[i]+v;
                    }
                    mlpbase.mlphessianbatch(network, xy, npoints, ref e, ref g, ref h);
                    v = 0.0;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        v += network.weights[i_]*network.weights[i_];
                    }
                    e = e+0.5*decay*v;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        g[i_] = g[i_] + decay*network.weights[i_];
                    }
                    for(k=0; k<=wcount-1; k++)
                    {
                        h[k,k] = h[k,k]+decay;
                    }
                    rep.nhess = rep.nhess+1;
                    
                    //
                    // Update lambda
                    //
                    lambdav = lambdav*lambdadown;
                    nu = 2;
                }
                
                //
                // update WBest
                //
                v = 0.0;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    v += network.weights[i_]*network.weights[i_];
                }
                e = 0.5*decay*v+mlpbase.mlperror(network, xy, npoints);
                if( (double)(e)<(double)(ebest) )
                {
                    ebest = e;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        wbest[i_] = network.weights[i_];
                    }
                }
            }
            
            //
            // copy WBest to output
            //
            for(i_=0; i_<=wcount-1;i_++)
            {
                network.weights[i_] = wbest[i_];
            }
        }


        /*************************************************************************
        Neural  network  training  using  L-BFGS  algorithm  with  regularization.
        Subroutine  trains  neural  network  with  restarts from random positions.
        Algorithm  is  well  suited  for  problems  of  any dimensionality (memory
        requirements and step complexity are linear by weights number).

        INPUT PARAMETERS:
            Network     -   neural network with initialized geometry
            XY          -   training set
            NPoints     -   training set size
            Decay       -   weight decay constant, >=0.001
                            Decay term 'Decay*||Weights||^2' is added to error
                            function.
                            If you don't know what Decay to choose, use 0.001.
            Restarts    -   number of restarts from random position, >0.
                            If you don't know what Restarts to choose, use 2.
            WStep       -   stopping criterion. Algorithm stops if  step  size  is
                            less than WStep. Recommended value - 0.01.  Zero  step
                            size means stopping after MaxIts iterations.
            MaxIts      -   stopping   criterion.  Algorithm  stops  after  MaxIts
                            iterations (NOT gradient  calculations).  Zero  MaxIts
                            means stopping when step is sufficiently small.

        OUTPUT PARAMETERS:
            Network     -   trained neural network.
            Info        -   return code:
                            * -8, if both WStep=0 and MaxIts=0
                            * -2, if there is a point with class number
                                  outside of [0..NOut-1].
                            * -1, if wrong parameters specified
                                  (NPoints<0, Restarts<1).
                            *  2, if task has been solved.
            Rep         -   training report

          -- ALGLIB --
             Copyright 09.12.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlptrainlbfgs(mlpbase.multilayerperceptron network,
            double[,] xy,
            int npoints,
            double decay,
            int restarts,
            double wstep,
            int maxits,
            ref int info,
            mlpreport rep)
        {
            int i = 0;
            int pass = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            double[] w = new double[0];
            double[] wbest = new double[0];
            double e = 0;
            double v = 0;
            double ebest = 0;
            minlbfgs.minlbfgsreport internalrep = new minlbfgs.minlbfgsreport();
            minlbfgs.minlbfgsstate state = new minlbfgs.minlbfgsstate();
            int i_ = 0;

            info = 0;

            
            //
            // Test inputs, parse flags, read network geometry
            //
            if( (double)(wstep)==(double)(0) & maxits==0 )
            {
                info = -8;
                return;
            }
            if( ((npoints<=0 | restarts<1) | (double)(wstep)<(double)(0)) | maxits<0 )
            {
                info = -1;
                return;
            }
            mlpbase.mlpproperties(network, ref nin, ref nout, ref wcount);
            if( mlpbase.mlpissoftmax(network) )
            {
                for(i=0; i<=npoints-1; i++)
                {
                    if( (int)Math.Round(xy[i,nin])<0 | (int)Math.Round(xy[i,nin])>=nout )
                    {
                        info = -2;
                        return;
                    }
                }
            }
            decay = Math.Max(decay, mindecay);
            info = 2;
            
            //
            // Prepare
            //
            mlpbase.mlpinitpreprocessor(network, xy, npoints);
            w = new double[wcount-1+1];
            wbest = new double[wcount-1+1];
            ebest = math.maxrealnumber;
            
            //
            // Multiple starts
            //
            rep.ncholesky = 0;
            rep.nhess = 0;
            rep.ngrad = 0;
            for(pass=1; pass<=restarts; pass++)
            {
                
                //
                // Process
                //
                mlpbase.mlprandomize(network);
                for(i_=0; i_<=wcount-1;i_++)
                {
                    w[i_] = network.weights[i_];
                }
                minlbfgs.minlbfgscreate(wcount, Math.Min(wcount, 10), w, state);
                minlbfgs.minlbfgssetcond(state, 0.0, 0.0, wstep, maxits);
                while( minlbfgs.minlbfgsiteration(state) )
                {
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        network.weights[i_] = state.x[i_];
                    }
                    mlpbase.mlpgradnbatch(network, xy, npoints, ref state.f, ref state.g);
                    v = 0.0;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        v += network.weights[i_]*network.weights[i_];
                    }
                    state.f = state.f+0.5*decay*v;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        state.g[i_] = state.g[i_] + decay*network.weights[i_];
                    }
                    rep.ngrad = rep.ngrad+1;
                }
                minlbfgs.minlbfgsresults(state, ref w, internalrep);
                for(i_=0; i_<=wcount-1;i_++)
                {
                    network.weights[i_] = w[i_];
                }
                
                //
                // Compare with best
                //
                v = 0.0;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    v += network.weights[i_]*network.weights[i_];
                }
                e = mlpbase.mlperrorn(network, xy, npoints)+0.5*decay*v;
                if( (double)(e)<(double)(ebest) )
                {
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        wbest[i_] = network.weights[i_];
                    }
                    ebest = e;
                }
            }
            
            //
            // The best network
            //
            for(i_=0; i_<=wcount-1;i_++)
            {
                network.weights[i_] = wbest[i_];
            }
        }


        /*************************************************************************
        Neural network training using early stopping (base algorithm - L-BFGS with
        regularization).

        INPUT PARAMETERS:
            Network     -   neural network with initialized geometry
            TrnXY       -   training set
            TrnSize     -   training set size
            ValXY       -   validation set
            ValSize     -   validation set size
            Decay       -   weight decay constant, >=0.001
                            Decay term 'Decay*||Weights||^2' is added to error
                            function.
                            If you don't know what Decay to choose, use 0.001.
            Restarts    -   number of restarts from random position, >0.
                            If you don't know what Restarts to choose, use 2.

        OUTPUT PARAMETERS:
            Network     -   trained neural network.
            Info        -   return code:
                            * -2, if there is a point with class number
                                  outside of [0..NOut-1].
                            * -1, if wrong parameters specified
                                  (NPoints<0, Restarts<1, ...).
                            *  2, task has been solved, stopping  criterion  met -
                                  sufficiently small step size.  Not expected  (we
                                  use  EARLY  stopping)  but  possible  and not an
                                  error.
                            *  6, task has been solved, stopping  criterion  met -
                                  increasing of validation set error.
            Rep         -   training report

        NOTE:

        Algorithm stops if validation set error increases for  a  long  enough  or
        step size is small enought  (there  are  task  where  validation  set  may
        decrease for eternity). In any case solution returned corresponds  to  the
        minimum of validation set error.

          -- ALGLIB --
             Copyright 10.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlptraines(mlpbase.multilayerperceptron network,
            double[,] trnxy,
            int trnsize,
            double[,] valxy,
            int valsize,
            double decay,
            int restarts,
            ref int info,
            mlpreport rep)
        {
            int i = 0;
            int pass = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            double[] w = new double[0];
            double[] wbest = new double[0];
            double e = 0;
            double v = 0;
            double ebest = 0;
            double[] wfinal = new double[0];
            double efinal = 0;
            int itbest = 0;
            minlbfgs.minlbfgsreport internalrep = new minlbfgs.minlbfgsreport();
            minlbfgs.minlbfgsstate state = new minlbfgs.minlbfgsstate();
            double wstep = 0;
            int i_ = 0;

            info = 0;

            wstep = 0.001;
            
            //
            // Test inputs, parse flags, read network geometry
            //
            if( ((trnsize<=0 | valsize<=0) | restarts<1) | (double)(decay)<(double)(0) )
            {
                info = -1;
                return;
            }
            mlpbase.mlpproperties(network, ref nin, ref nout, ref wcount);
            if( mlpbase.mlpissoftmax(network) )
            {
                for(i=0; i<=trnsize-1; i++)
                {
                    if( (int)Math.Round(trnxy[i,nin])<0 | (int)Math.Round(trnxy[i,nin])>=nout )
                    {
                        info = -2;
                        return;
                    }
                }
                for(i=0; i<=valsize-1; i++)
                {
                    if( (int)Math.Round(valxy[i,nin])<0 | (int)Math.Round(valxy[i,nin])>=nout )
                    {
                        info = -2;
                        return;
                    }
                }
            }
            info = 2;
            
            //
            // Prepare
            //
            mlpbase.mlpinitpreprocessor(network, trnxy, trnsize);
            w = new double[wcount-1+1];
            wbest = new double[wcount-1+1];
            wfinal = new double[wcount-1+1];
            efinal = math.maxrealnumber;
            for(i=0; i<=wcount-1; i++)
            {
                wfinal[i] = 0;
            }
            
            //
            // Multiple starts
            //
            rep.ncholesky = 0;
            rep.nhess = 0;
            rep.ngrad = 0;
            for(pass=1; pass<=restarts; pass++)
            {
                
                //
                // Process
                //
                mlpbase.mlprandomize(network);
                ebest = mlpbase.mlperror(network, valxy, valsize);
                for(i_=0; i_<=wcount-1;i_++)
                {
                    wbest[i_] = network.weights[i_];
                }
                itbest = 0;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    w[i_] = network.weights[i_];
                }
                minlbfgs.minlbfgscreate(wcount, Math.Min(wcount, 10), w, state);
                minlbfgs.minlbfgssetcond(state, 0.0, 0.0, wstep, 0);
                minlbfgs.minlbfgssetxrep(state, true);
                while( minlbfgs.minlbfgsiteration(state) )
                {
                    
                    //
                    // Calculate gradient
                    //
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        network.weights[i_] = state.x[i_];
                    }
                    mlpbase.mlpgradnbatch(network, trnxy, trnsize, ref state.f, ref state.g);
                    v = 0.0;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        v += network.weights[i_]*network.weights[i_];
                    }
                    state.f = state.f+0.5*decay*v;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        state.g[i_] = state.g[i_] + decay*network.weights[i_];
                    }
                    rep.ngrad = rep.ngrad+1;
                    
                    //
                    // Validation set
                    //
                    if( state.xupdated )
                    {
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            network.weights[i_] = w[i_];
                        }
                        e = mlpbase.mlperror(network, valxy, valsize);
                        if( (double)(e)<(double)(ebest) )
                        {
                            ebest = e;
                            for(i_=0; i_<=wcount-1;i_++)
                            {
                                wbest[i_] = network.weights[i_];
                            }
                            itbest = internalrep.iterationscount;
                        }
                        if( internalrep.iterationscount>30 & (double)(internalrep.iterationscount)>(double)(1.5*itbest) )
                        {
                            info = 6;
                            break;
                        }
                    }
                }
                minlbfgs.minlbfgsresults(state, ref w, internalrep);
                
                //
                // Compare with final answer
                //
                if( (double)(ebest)<(double)(efinal) )
                {
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        wfinal[i_] = wbest[i_];
                    }
                    efinal = ebest;
                }
            }
            
            //
            // The best network
            //
            for(i_=0; i_<=wcount-1;i_++)
            {
                network.weights[i_] = wfinal[i_];
            }
        }


        /*************************************************************************
        Cross-validation estimate of generalization error.

        Base algorithm - L-BFGS.

        INPUT PARAMETERS:
            Network     -   neural network with initialized geometry.   Network is
                            not changed during cross-validation -  it is used only
                            as a representative of its architecture.
            XY          -   training set.
            SSize       -   training set size
            Decay       -   weight  decay, same as in MLPTrainLBFGS
            Restarts    -   number of restarts, >0.
                            restarts are counted for each partition separately, so
                            total number of restarts will be Restarts*FoldsCount.
            WStep       -   stopping criterion, same as in MLPTrainLBFGS
            MaxIts      -   stopping criterion, same as in MLPTrainLBFGS
            FoldsCount  -   number of folds in k-fold cross-validation,
                            2<=FoldsCount<=SSize.
                            recommended value: 10.

        OUTPUT PARAMETERS:
            Info        -   return code, same as in MLPTrainLBFGS
            Rep         -   report, same as in MLPTrainLM/MLPTrainLBFGS
            CVRep       -   generalization error estimates

          -- ALGLIB --
             Copyright 09.12.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpkfoldcvlbfgs(mlpbase.multilayerperceptron network,
            double[,] xy,
            int npoints,
            double decay,
            int restarts,
            double wstep,
            int maxits,
            int foldscount,
            ref int info,
            mlpreport rep,
            mlpcvreport cvrep)
        {
            info = 0;

            mlpkfoldcvgeneral(network, xy, npoints, decay, restarts, foldscount, false, wstep, maxits, ref info, rep, cvrep);
        }


        /*************************************************************************
        Cross-validation estimate of generalization error.

        Base algorithm - Levenberg-Marquardt.

        INPUT PARAMETERS:
            Network     -   neural network with initialized geometry.   Network is
                            not changed during cross-validation -  it is used only
                            as a representative of its architecture.
            XY          -   training set.
            SSize       -   training set size
            Decay       -   weight  decay, same as in MLPTrainLBFGS
            Restarts    -   number of restarts, >0.
                            restarts are counted for each partition separately, so
                            total number of restarts will be Restarts*FoldsCount.
            FoldsCount  -   number of folds in k-fold cross-validation,
                            2<=FoldsCount<=SSize.
                            recommended value: 10.

        OUTPUT PARAMETERS:
            Info        -   return code, same as in MLPTrainLBFGS
            Rep         -   report, same as in MLPTrainLM/MLPTrainLBFGS
            CVRep       -   generalization error estimates

          -- ALGLIB --
             Copyright 09.12.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpkfoldcvlm(mlpbase.multilayerperceptron network,
            double[,] xy,
            int npoints,
            double decay,
            int restarts,
            int foldscount,
            ref int info,
            mlpreport rep,
            mlpcvreport cvrep)
        {
            info = 0;

            mlpkfoldcvgeneral(network, xy, npoints, decay, restarts, foldscount, true, 0.0, 0, ref info, rep, cvrep);
        }


        /*************************************************************************
        Internal cross-validation subroutine
        *************************************************************************/
        private static void mlpkfoldcvgeneral(mlpbase.multilayerperceptron n,
            double[,] xy,
            int npoints,
            double decay,
            int restarts,
            int foldscount,
            bool lmalgorithm,
            double wstep,
            int maxits,
            ref int info,
            mlpreport rep,
            mlpcvreport cvrep)
        {
            int i = 0;
            int fold = 0;
            int j = 0;
            int k = 0;
            mlpbase.multilayerperceptron network = new mlpbase.multilayerperceptron();
            int nin = 0;
            int nout = 0;
            int rowlen = 0;
            int wcount = 0;
            int nclasses = 0;
            int tssize = 0;
            int cvssize = 0;
            double[,] cvset = new double[0,0];
            double[,] testset = new double[0,0];
            int[] folds = new int[0];
            int relcnt = 0;
            mlpreport internalrep = new mlpreport();
            double[] x = new double[0];
            double[] y = new double[0];
            int i_ = 0;

            info = 0;

            
            //
            // Read network geometry, test parameters
            //
            mlpbase.mlpproperties(n, ref nin, ref nout, ref wcount);
            if( mlpbase.mlpissoftmax(n) )
            {
                nclasses = nout;
                rowlen = nin+1;
            }
            else
            {
                nclasses = -nout;
                rowlen = nin+nout;
            }
            if( (npoints<=0 | foldscount<2) | foldscount>npoints )
            {
                info = -1;
                return;
            }
            mlpbase.mlpcopy(n, network);
            
            //
            // K-fold out cross-validation.
            // First, estimate generalization error
            //
            testset = new double[npoints-1+1, rowlen-1+1];
            cvset = new double[npoints-1+1, rowlen-1+1];
            x = new double[nin-1+1];
            y = new double[nout-1+1];
            mlpkfoldsplit(xy, npoints, nclasses, foldscount, false, ref folds);
            cvrep.relclserror = 0;
            cvrep.avgce = 0;
            cvrep.rmserror = 0;
            cvrep.avgerror = 0;
            cvrep.avgrelerror = 0;
            rep.ngrad = 0;
            rep.nhess = 0;
            rep.ncholesky = 0;
            relcnt = 0;
            for(fold=0; fold<=foldscount-1; fold++)
            {
                
                //
                // Separate set
                //
                tssize = 0;
                cvssize = 0;
                for(i=0; i<=npoints-1; i++)
                {
                    if( folds[i]==fold )
                    {
                        for(i_=0; i_<=rowlen-1;i_++)
                        {
                            testset[tssize,i_] = xy[i,i_];
                        }
                        tssize = tssize+1;
                    }
                    else
                    {
                        for(i_=0; i_<=rowlen-1;i_++)
                        {
                            cvset[cvssize,i_] = xy[i,i_];
                        }
                        cvssize = cvssize+1;
                    }
                }
                
                //
                // Train on CV training set
                //
                if( lmalgorithm )
                {
                    mlptrainlm(network, cvset, cvssize, decay, restarts, ref info, internalrep);
                }
                else
                {
                    mlptrainlbfgs(network, cvset, cvssize, decay, restarts, wstep, maxits, ref info, internalrep);
                }
                if( info<0 )
                {
                    cvrep.relclserror = 0;
                    cvrep.avgce = 0;
                    cvrep.rmserror = 0;
                    cvrep.avgerror = 0;
                    cvrep.avgrelerror = 0;
                    return;
                }
                rep.ngrad = rep.ngrad+internalrep.ngrad;
                rep.nhess = rep.nhess+internalrep.nhess;
                rep.ncholesky = rep.ncholesky+internalrep.ncholesky;
                
                //
                // Estimate error using CV test set
                //
                if( mlpbase.mlpissoftmax(network) )
                {
                    
                    //
                    // classification-only code
                    //
                    cvrep.relclserror = cvrep.relclserror+mlpbase.mlpclserror(network, testset, tssize);
                    cvrep.avgce = cvrep.avgce+mlpbase.mlperrorn(network, testset, tssize);
                }
                for(i=0; i<=tssize-1; i++)
                {
                    for(i_=0; i_<=nin-1;i_++)
                    {
                        x[i_] = testset[i,i_];
                    }
                    mlpbase.mlpprocess(network, x, ref y);
                    if( mlpbase.mlpissoftmax(network) )
                    {
                        
                        //
                        // Classification-specific code
                        //
                        k = (int)Math.Round(testset[i,nin]);
                        for(j=0; j<=nout-1; j++)
                        {
                            if( j==k )
                            {
                                cvrep.rmserror = cvrep.rmserror+math.sqr(y[j]-1);
                                cvrep.avgerror = cvrep.avgerror+Math.Abs(y[j]-1);
                                cvrep.avgrelerror = cvrep.avgrelerror+Math.Abs(y[j]-1);
                                relcnt = relcnt+1;
                            }
                            else
                            {
                                cvrep.rmserror = cvrep.rmserror+math.sqr(y[j]);
                                cvrep.avgerror = cvrep.avgerror+Math.Abs(y[j]);
                            }
                        }
                    }
                    else
                    {
                        
                        //
                        // Regression-specific code
                        //
                        for(j=0; j<=nout-1; j++)
                        {
                            cvrep.rmserror = cvrep.rmserror+math.sqr(y[j]-testset[i,nin+j]);
                            cvrep.avgerror = cvrep.avgerror+Math.Abs(y[j]-testset[i,nin+j]);
                            if( (double)(testset[i,nin+j])!=(double)(0) )
                            {
                                cvrep.avgrelerror = cvrep.avgrelerror+Math.Abs((y[j]-testset[i,nin+j])/testset[i,nin+j]);
                                relcnt = relcnt+1;
                            }
                        }
                    }
                }
            }
            if( mlpbase.mlpissoftmax(network) )
            {
                cvrep.relclserror = cvrep.relclserror/npoints;
                cvrep.avgce = cvrep.avgce/(Math.Log(2)*npoints);
            }
            cvrep.rmserror = Math.Sqrt(cvrep.rmserror/(npoints*nout));
            cvrep.avgerror = cvrep.avgerror/(npoints*nout);
            cvrep.avgrelerror = cvrep.avgrelerror/relcnt;
            info = 1;
        }


        /*************************************************************************
        Subroutine prepares K-fold split of the training set.

        NOTES:
            "NClasses>0" means that we have classification task.
            "NClasses<0" means regression task with -NClasses real outputs.
        *************************************************************************/
        private static void mlpkfoldsplit(double[,] xy,
            int npoints,
            int nclasses,
            int foldscount,
            bool stratifiedsplits,
            ref int[] folds)
        {
            int i = 0;
            int j = 0;
            int k = 0;

            folds = new int[0];

            
            //
            // test parameters
            //
            ap.assert(npoints>0, "MLPKFoldSplit: wrong NPoints!");
            ap.assert(nclasses>1 | nclasses<0, "MLPKFoldSplit: wrong NClasses!");
            ap.assert(foldscount>=2 & foldscount<=npoints, "MLPKFoldSplit: wrong FoldsCount!");
            ap.assert(!stratifiedsplits, "MLPKFoldSplit: stratified splits are not supported!");
            
            //
            // Folds
            //
            folds = new int[npoints-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                folds[i] = i*foldscount/npoints;
            }
            for(i=0; i<=npoints-2; i++)
            {
                j = i+math.randominteger(npoints-i);
                if( j!=i )
                {
                    k = folds[i];
                    folds[i] = folds[j];
                    folds[j] = k;
                }
            }
        }


    }
    public class mlpe
    {
        /*************************************************************************
        Neural networks ensemble
        *************************************************************************/
        public class mlpensemble
        {
            public int[] structinfo;
            public int ensemblesize;
            public int nin;
            public int nout;
            public int wcount;
            public bool issoftmax;
            public bool postprocessing;
            public double[] weights;
            public double[] columnmeans;
            public double[] columnsigmas;
            public int serializedlen;
            public double[] serializedmlp;
            public double[] tmpweights;
            public double[] tmpmeans;
            public double[] tmpsigmas;
            public double[] neurons;
            public double[] dfdnet;
            public double[] y;
            public mlpensemble()
            {
                structinfo = new int[0];
                weights = new double[0];
                columnmeans = new double[0];
                columnsigmas = new double[0];
                serializedmlp = new double[0];
                tmpweights = new double[0];
                tmpmeans = new double[0];
                tmpsigmas = new double[0];
                neurons = new double[0];
                dfdnet = new double[0];
                y = new double[0];
            }
        };




        public const int mlpntotaloffset = 3;
        public const int mlpevnum = 9;


        /*************************************************************************
        Like MLPCreate0, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreate0(int nin,
            int nout,
            int ensemblesize,
            mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreate0(nin, nout, net);
            mlpecreatefromnetwork(net, ensemblesize, ensemble);
        }


        /*************************************************************************
        Like MLPCreate1, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreate1(int nin,
            int nhid,
            int nout,
            int ensemblesize,
            mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreate1(nin, nhid, nout, net);
            mlpecreatefromnetwork(net, ensemblesize, ensemble);
        }


        /*************************************************************************
        Like MLPCreate2, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreate2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            int ensemblesize,
            mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreate2(nin, nhid1, nhid2, nout, net);
            mlpecreatefromnetwork(net, ensemblesize, ensemble);
        }


        /*************************************************************************
        Like MLPCreateB0, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreateb0(int nin,
            int nout,
            double b,
            double d,
            int ensemblesize,
            mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreateb0(nin, nout, b, d, net);
            mlpecreatefromnetwork(net, ensemblesize, ensemble);
        }


        /*************************************************************************
        Like MLPCreateB1, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreateb1(int nin,
            int nhid,
            int nout,
            double b,
            double d,
            int ensemblesize,
            mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreateb1(nin, nhid, nout, b, d, net);
            mlpecreatefromnetwork(net, ensemblesize, ensemble);
        }


        /*************************************************************************
        Like MLPCreateB2, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreateb2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            double b,
            double d,
            int ensemblesize,
            mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreateb2(nin, nhid1, nhid2, nout, b, d, net);
            mlpecreatefromnetwork(net, ensemblesize, ensemble);
        }


        /*************************************************************************
        Like MLPCreateR0, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreater0(int nin,
            int nout,
            double a,
            double b,
            int ensemblesize,
            mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreater0(nin, nout, a, b, net);
            mlpecreatefromnetwork(net, ensemblesize, ensemble);
        }


        /*************************************************************************
        Like MLPCreateR1, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreater1(int nin,
            int nhid,
            int nout,
            double a,
            double b,
            int ensemblesize,
            mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreater1(nin, nhid, nout, a, b, net);
            mlpecreatefromnetwork(net, ensemblesize, ensemble);
        }


        /*************************************************************************
        Like MLPCreateR2, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreater2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            double a,
            double b,
            int ensemblesize,
            mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreater2(nin, nhid1, nhid2, nout, a, b, net);
            mlpecreatefromnetwork(net, ensemblesize, ensemble);
        }


        /*************************************************************************
        Like MLPCreateC0, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreatec0(int nin,
            int nout,
            int ensemblesize,
            mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreatec0(nin, nout, net);
            mlpecreatefromnetwork(net, ensemblesize, ensemble);
        }


        /*************************************************************************
        Like MLPCreateC1, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreatec1(int nin,
            int nhid,
            int nout,
            int ensemblesize,
            mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreatec1(nin, nhid, nout, net);
            mlpecreatefromnetwork(net, ensemblesize, ensemble);
        }


        /*************************************************************************
        Like MLPCreateC2, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreatec2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            int ensemblesize,
            mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreatec2(nin, nhid1, nhid2, nout, net);
            mlpecreatefromnetwork(net, ensemblesize, ensemble);
        }


        /*************************************************************************
        Creates ensemble from network. Only network geometry is copied.

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreatefromnetwork(mlpbase.multilayerperceptron network,
            int ensemblesize,
            mlpensemble ensemble)
        {
            int i = 0;
            int ccount = 0;
            int i_ = 0;
            int i1_ = 0;

            ap.assert(ensemblesize>0, "MLPECreate: incorrect ensemble size!");
            
            //
            // network properties
            //
            mlpbase.mlpproperties(network, ref ensemble.nin, ref ensemble.nout, ref ensemble.wcount);
            if( mlpbase.mlpissoftmax(network) )
            {
                ccount = ensemble.nin;
            }
            else
            {
                ccount = ensemble.nin+ensemble.nout;
            }
            ensemble.postprocessing = false;
            ensemble.issoftmax = mlpbase.mlpissoftmax(network);
            ensemble.ensemblesize = ensemblesize;
            
            //
            // structure information
            //
            ensemble.structinfo = new int[network.structinfo[0]-1+1];
            for(i=0; i<=network.structinfo[0]-1; i++)
            {
                ensemble.structinfo[i] = network.structinfo[i];
            }
            
            //
            // weights, means, sigmas
            //
            ensemble.weights = new double[ensemblesize*ensemble.wcount-1+1];
            ensemble.columnmeans = new double[ensemblesize*ccount-1+1];
            ensemble.columnsigmas = new double[ensemblesize*ccount-1+1];
            for(i=0; i<=ensemblesize*ensemble.wcount-1; i++)
            {
                ensemble.weights[i] = math.randomreal()-0.5;
            }
            for(i=0; i<=ensemblesize-1; i++)
            {
                i1_ = (0) - (i*ccount);
                for(i_=i*ccount; i_<=(i+1)*ccount-1;i_++)
                {
                    ensemble.columnmeans[i_] = network.columnmeans[i_+i1_];
                }
                i1_ = (0) - (i*ccount);
                for(i_=i*ccount; i_<=(i+1)*ccount-1;i_++)
                {
                    ensemble.columnsigmas[i_] = network.columnsigmas[i_+i1_];
                }
            }
            
            //
            // serialized part
            //
            mlpbase.mlpserialize(network, ref ensemble.serializedmlp, ref ensemble.serializedlen);
            
            //
            // temporaries, internal buffers
            //
            ensemble.tmpweights = new double[ensemble.wcount-1+1];
            ensemble.tmpmeans = new double[ccount-1+1];
            ensemble.tmpsigmas = new double[ccount-1+1];
            ensemble.neurons = new double[ensemble.structinfo[mlpntotaloffset]-1+1];
            ensemble.dfdnet = new double[ensemble.structinfo[mlpntotaloffset]-1+1];
            ensemble.y = new double[ensemble.nout-1+1];
        }


        /*************************************************************************
        Copying of MLPEnsemble strucure

        INPUT PARAMETERS:
            Ensemble1 -   original

        OUTPUT PARAMETERS:
            Ensemble2 -   copy

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecopy(mlpensemble ensemble1,
            mlpensemble ensemble2)
        {
            int i = 0;
            int ssize = 0;
            int ccount = 0;
            int ntotal = 0;
            int i_ = 0;

            
            //
            // Unload info
            //
            ssize = ensemble1.structinfo[0];
            if( ensemble1.issoftmax )
            {
                ccount = ensemble1.nin;
            }
            else
            {
                ccount = ensemble1.nin+ensemble1.nout;
            }
            ntotal = ensemble1.structinfo[mlpntotaloffset];
            
            //
            // Allocate space
            //
            ensemble2.structinfo = new int[ssize-1+1];
            ensemble2.weights = new double[ensemble1.ensemblesize*ensemble1.wcount-1+1];
            ensemble2.columnmeans = new double[ensemble1.ensemblesize*ccount-1+1];
            ensemble2.columnsigmas = new double[ensemble1.ensemblesize*ccount-1+1];
            ensemble2.tmpweights = new double[ensemble1.wcount-1+1];
            ensemble2.tmpmeans = new double[ccount-1+1];
            ensemble2.tmpsigmas = new double[ccount-1+1];
            ensemble2.serializedmlp = new double[ensemble1.serializedlen-1+1];
            ensemble2.neurons = new double[ntotal-1+1];
            ensemble2.dfdnet = new double[ntotal-1+1];
            ensemble2.y = new double[ensemble1.nout-1+1];
            
            //
            // Copy
            //
            ensemble2.nin = ensemble1.nin;
            ensemble2.nout = ensemble1.nout;
            ensemble2.wcount = ensemble1.wcount;
            ensemble2.ensemblesize = ensemble1.ensemblesize;
            ensemble2.issoftmax = ensemble1.issoftmax;
            ensemble2.postprocessing = ensemble1.postprocessing;
            ensemble2.serializedlen = ensemble1.serializedlen;
            for(i=0; i<=ssize-1; i++)
            {
                ensemble2.structinfo[i] = ensemble1.structinfo[i];
            }
            for(i_=0; i_<=ensemble1.ensemblesize*ensemble1.wcount-1;i_++)
            {
                ensemble2.weights[i_] = ensemble1.weights[i_];
            }
            for(i_=0; i_<=ensemble1.ensemblesize*ccount-1;i_++)
            {
                ensemble2.columnmeans[i_] = ensemble1.columnmeans[i_];
            }
            for(i_=0; i_<=ensemble1.ensemblesize*ccount-1;i_++)
            {
                ensemble2.columnsigmas[i_] = ensemble1.columnsigmas[i_];
            }
            for(i_=0; i_<=ensemble1.serializedlen-1;i_++)
            {
                ensemble2.serializedmlp[i_] = ensemble1.serializedmlp[i_];
            }
        }


        /*************************************************************************
        Serialization of MLPEnsemble strucure

        INPUT PARAMETERS:
            Ensemble-   original

        OUTPUT PARAMETERS:
            RA      -   array of real numbers which stores ensemble,
                        array[0..RLen-1]
            RLen    -   RA lenght

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpeserialize(mlpensemble ensemble,
            ref double[] ra,
            ref int rlen)
        {
            int i = 0;
            int ssize = 0;
            int ntotal = 0;
            int ccount = 0;
            int hsize = 0;
            int offs = 0;
            int i_ = 0;
            int i1_ = 0;

            ra = new double[0];
            rlen = 0;

            hsize = 13;
            ssize = ensemble.structinfo[0];
            if( ensemble.issoftmax )
            {
                ccount = ensemble.nin;
            }
            else
            {
                ccount = ensemble.nin+ensemble.nout;
            }
            ntotal = ensemble.structinfo[mlpntotaloffset];
            rlen = hsize+ssize+ensemble.ensemblesize*ensemble.wcount+2*ccount*ensemble.ensemblesize+ensemble.serializedlen;
            
            //
            //  RA format:
            //  [0]     RLen
            //  [1]     Version (MLPEVNum)
            //  [2]     EnsembleSize
            //  [3]     NIn
            //  [4]     NOut
            //  [5]     WCount
            //  [6]     IsSoftmax 0/1
            //  [7]     PostProcessing 0/1
            //  [8]     sizeof(StructInfo)
            //  [9]     NTotal (sizeof(Neurons), sizeof(DFDNET))
            //  [10]    CCount (sizeof(ColumnMeans), sizeof(ColumnSigmas))
            //  [11]    data offset
            //  [12]    SerializedLen
            //
            //  [..]    StructInfo
            //  [..]    Weights
            //  [..]    ColumnMeans
            //  [..]    ColumnSigmas
            //
            ra = new double[rlen-1+1];
            ra[0] = rlen;
            ra[1] = mlpevnum;
            ra[2] = ensemble.ensemblesize;
            ra[3] = ensemble.nin;
            ra[4] = ensemble.nout;
            ra[5] = ensemble.wcount;
            if( ensemble.issoftmax )
            {
                ra[6] = 1;
            }
            else
            {
                ra[6] = 0;
            }
            if( ensemble.postprocessing )
            {
                ra[7] = 1;
            }
            else
            {
                ra[7] = 9;
            }
            ra[8] = ssize;
            ra[9] = ntotal;
            ra[10] = ccount;
            ra[11] = hsize;
            ra[12] = ensemble.serializedlen;
            offs = hsize;
            for(i=offs; i<=offs+ssize-1; i++)
            {
                ra[i] = ensemble.structinfo[i-offs];
            }
            offs = offs+ssize;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+ensemble.ensemblesize*ensemble.wcount-1;i_++)
            {
                ra[i_] = ensemble.weights[i_+i1_];
            }
            offs = offs+ensemble.ensemblesize*ensemble.wcount;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+ensemble.ensemblesize*ccount-1;i_++)
            {
                ra[i_] = ensemble.columnmeans[i_+i1_];
            }
            offs = offs+ensemble.ensemblesize*ccount;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+ensemble.ensemblesize*ccount-1;i_++)
            {
                ra[i_] = ensemble.columnsigmas[i_+i1_];
            }
            offs = offs+ensemble.ensemblesize*ccount;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+ensemble.serializedlen-1;i_++)
            {
                ra[i_] = ensemble.serializedmlp[i_+i1_];
            }
            offs = offs+ensemble.serializedlen;
        }


        /*************************************************************************
        Unserialization of MLPEnsemble strucure

        INPUT PARAMETERS:
            RA      -   real array which stores ensemble

        OUTPUT PARAMETERS:
            Ensemble-   restored structure

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpeunserialize(double[] ra,
            mlpensemble ensemble)
        {
            int i = 0;
            int ssize = 0;
            int ntotal = 0;
            int ccount = 0;
            int hsize = 0;
            int offs = 0;
            int i_ = 0;
            int i1_ = 0;

            ap.assert((int)Math.Round(ra[1])==mlpevnum, "MLPEUnserialize: incorrect array!");
            
            //
            // load info
            //
            hsize = 13;
            ensemble.ensemblesize = (int)Math.Round(ra[2]);
            ensemble.nin = (int)Math.Round(ra[3]);
            ensemble.nout = (int)Math.Round(ra[4]);
            ensemble.wcount = (int)Math.Round(ra[5]);
            ensemble.issoftmax = (int)Math.Round(ra[6])==1;
            ensemble.postprocessing = (int)Math.Round(ra[7])==1;
            ssize = (int)Math.Round(ra[8]);
            ntotal = (int)Math.Round(ra[9]);
            ccount = (int)Math.Round(ra[10]);
            offs = (int)Math.Round(ra[11]);
            ensemble.serializedlen = (int)Math.Round(ra[12]);
            
            //
            //  Allocate arrays
            //
            ensemble.structinfo = new int[ssize-1+1];
            ensemble.weights = new double[ensemble.ensemblesize*ensemble.wcount-1+1];
            ensemble.columnmeans = new double[ensemble.ensemblesize*ccount-1+1];
            ensemble.columnsigmas = new double[ensemble.ensemblesize*ccount-1+1];
            ensemble.tmpweights = new double[ensemble.wcount-1+1];
            ensemble.tmpmeans = new double[ccount-1+1];
            ensemble.tmpsigmas = new double[ccount-1+1];
            ensemble.neurons = new double[ntotal-1+1];
            ensemble.dfdnet = new double[ntotal-1+1];
            ensemble.serializedmlp = new double[ensemble.serializedlen-1+1];
            ensemble.y = new double[ensemble.nout-1+1];
            
            //
            // load data
            //
            for(i=offs; i<=offs+ssize-1; i++)
            {
                ensemble.structinfo[i-offs] = (int)Math.Round(ra[i]);
            }
            offs = offs+ssize;
            i1_ = (offs) - (0);
            for(i_=0; i_<=ensemble.ensemblesize*ensemble.wcount-1;i_++)
            {
                ensemble.weights[i_] = ra[i_+i1_];
            }
            offs = offs+ensemble.ensemblesize*ensemble.wcount;
            i1_ = (offs) - (0);
            for(i_=0; i_<=ensemble.ensemblesize*ccount-1;i_++)
            {
                ensemble.columnmeans[i_] = ra[i_+i1_];
            }
            offs = offs+ensemble.ensemblesize*ccount;
            i1_ = (offs) - (0);
            for(i_=0; i_<=ensemble.ensemblesize*ccount-1;i_++)
            {
                ensemble.columnsigmas[i_] = ra[i_+i1_];
            }
            offs = offs+ensemble.ensemblesize*ccount;
            i1_ = (offs) - (0);
            for(i_=0; i_<=ensemble.serializedlen-1;i_++)
            {
                ensemble.serializedmlp[i_] = ra[i_+i1_];
            }
            offs = offs+ensemble.serializedlen;
        }


        /*************************************************************************
        Randomization of MLP ensemble

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlperandomize(mlpensemble ensemble)
        {
            int i = 0;

            for(i=0; i<=ensemble.ensemblesize*ensemble.wcount-1; i++)
            {
                ensemble.weights[i] = math.randomreal()-0.5;
            }
        }


        /*************************************************************************
        Return ensemble properties (number of inputs and outputs).

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpeproperties(mlpensemble ensemble,
            ref int nin,
            ref int nout)
        {
            nin = 0;
            nout = 0;

            nin = ensemble.nin;
            nout = ensemble.nout;
        }


        /*************************************************************************
        Return normalization type (whether ensemble is SOFTMAX-normalized or not).

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool mlpeissoftmax(mlpensemble ensemble)
        {
            bool result = new bool();

            result = ensemble.issoftmax;
            return result;
        }


        /*************************************************************************
        Procesing

        INPUT PARAMETERS:
            Ensemble-   neural networks ensemble
            X       -   input vector,  array[0..NIn-1].
            Y       -   (possibly) preallocated buffer; if size of Y is less than
                        NOut, it will be reallocated. If it is large enough, it
                        is NOT reallocated, so we can save some time on reallocation.


        OUTPUT PARAMETERS:
            Y       -   result. Regression estimate when solving regression  task,
                        vector of posterior probabilities for classification task.

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpeprocess(mlpensemble ensemble,
            double[] x,
            ref double[] y)
        {
            int i = 0;
            int es = 0;
            int wc = 0;
            int cc = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            if( ap.len(y)<ensemble.nout )
            {
                y = new double[ensemble.nout];
            }
            es = ensemble.ensemblesize;
            wc = ensemble.wcount;
            if( ensemble.issoftmax )
            {
                cc = ensemble.nin;
            }
            else
            {
                cc = ensemble.nin+ensemble.nout;
            }
            v = (double)1/(double)es;
            for(i=0; i<=ensemble.nout-1; i++)
            {
                y[i] = 0;
            }
            for(i=0; i<=es-1; i++)
            {
                i1_ = (i*wc) - (0);
                for(i_=0; i_<=wc-1;i_++)
                {
                    ensemble.tmpweights[i_] = ensemble.weights[i_+i1_];
                }
                i1_ = (i*cc) - (0);
                for(i_=0; i_<=cc-1;i_++)
                {
                    ensemble.tmpmeans[i_] = ensemble.columnmeans[i_+i1_];
                }
                i1_ = (i*cc) - (0);
                for(i_=0; i_<=cc-1;i_++)
                {
                    ensemble.tmpsigmas[i_] = ensemble.columnsigmas[i_+i1_];
                }
                mlpbase.mlpinternalprocessvector(ensemble.structinfo, ensemble.tmpweights, ensemble.tmpmeans, ensemble.tmpsigmas, ref ensemble.neurons, ref ensemble.dfdnet, x, ref ensemble.y);
                for(i_=0; i_<=ensemble.nout-1;i_++)
                {
                    y[i_] = y[i_] + v*ensemble.y[i_];
                }
            }
        }


        /*************************************************************************
        'interactive'  variant  of  MLPEProcess  for  languages  like Python which
        support constructs like "Y = MLPEProcess(LM,X)" and interactive mode of the
        interpreter

        This function allocates new array on each call,  so  it  is  significantly
        slower than its 'non-interactive' counterpart, but it is  more  convenient
        when you call it from command line.

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpeprocessi(mlpensemble ensemble,
            double[] x,
            ref double[] y)
        {
            y = new double[0];

            mlpeprocess(ensemble, x, ref y);
        }


        /*************************************************************************
        Relative classification error on the test set

        INPUT PARAMETERS:
            Ensemble-   ensemble
            XY      -   test set
            NPoints -   test set size

        RESULT:
            percent of incorrectly classified cases.
            Works both for classifier betwork and for regression networks which
        are used as classifiers.

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double mlperelclserror(mlpensemble ensemble,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            mlpeallerrors(ensemble, xy, npoints, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = relcls;
            return result;
        }


        /*************************************************************************
        Average cross-entropy (in bits per element) on the test set

        INPUT PARAMETERS:
            Ensemble-   ensemble
            XY      -   test set
            NPoints -   test set size

        RESULT:
            CrossEntropy/(NPoints*LN(2)).
            Zero if ensemble solves regression task.

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double mlpeavgce(mlpensemble ensemble,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            mlpeallerrors(ensemble, xy, npoints, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = avgce;
            return result;
        }


        /*************************************************************************
        RMS error on the test set

        INPUT PARAMETERS:
            Ensemble-   ensemble
            XY      -   test set
            NPoints -   test set size

        RESULT:
            root mean square error.
            Its meaning for regression task is obvious. As for classification task
        RMS error means error when estimating posterior probabilities.

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double mlpermserror(mlpensemble ensemble,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            mlpeallerrors(ensemble, xy, npoints, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = rms;
            return result;
        }


        /*************************************************************************
        Average error on the test set

        INPUT PARAMETERS:
            Ensemble-   ensemble
            XY      -   test set
            NPoints -   test set size

        RESULT:
            Its meaning for regression task is obvious. As for classification task
        it means average error when estimating posterior probabilities.

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double mlpeavgerror(mlpensemble ensemble,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            mlpeallerrors(ensemble, xy, npoints, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = avg;
            return result;
        }


        /*************************************************************************
        Average relative error on the test set

        INPUT PARAMETERS:
            Ensemble-   ensemble
            XY      -   test set
            NPoints -   test set size

        RESULT:
            Its meaning for regression task is obvious. As for classification task
        it means average relative error when estimating posterior probabilities.

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double mlpeavgrelerror(mlpensemble ensemble,
            double[,] xy,
            int npoints)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            mlpeallerrors(ensemble, xy, npoints, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = avgrel;
            return result;
        }


        /*************************************************************************
        Training neural networks ensemble using  bootstrap  aggregating (bagging).
        Modified Levenberg-Marquardt algorithm is used as base training method.

        INPUT PARAMETERS:
            Ensemble    -   model with initialized geometry
            XY          -   training set
            NPoints     -   training set size
            Decay       -   weight decay coefficient, >=0.001
            Restarts    -   restarts, >0.

        OUTPUT PARAMETERS:
            Ensemble    -   trained model
            Info        -   return code:
                            * -2, if there is a point with class number
                                  outside of [0..NClasses-1].
                            * -1, if incorrect parameters was passed
                                  (NPoints<0, Restarts<1).
                            *  2, if task has been solved.
            Rep         -   training report.
            OOBErrors   -   out-of-bag generalization error estimate

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpebagginglm(mlpensemble ensemble,
            double[,] xy,
            int npoints,
            double decay,
            int restarts,
            ref int info,
            mlptrain.mlpreport rep,
            mlptrain.mlpcvreport ooberrors)
        {
            info = 0;

            mlpebagginginternal(ensemble, xy, npoints, decay, restarts, 0.0, 0, true, ref info, rep, ooberrors);
        }


        /*************************************************************************
        Training neural networks ensemble using  bootstrap  aggregating (bagging).
        L-BFGS algorithm is used as base training method.

        INPUT PARAMETERS:
            Ensemble    -   model with initialized geometry
            XY          -   training set
            NPoints     -   training set size
            Decay       -   weight decay coefficient, >=0.001
            Restarts    -   restarts, >0.
            WStep       -   stopping criterion, same as in MLPTrainLBFGS
            MaxIts      -   stopping criterion, same as in MLPTrainLBFGS

        OUTPUT PARAMETERS:
            Ensemble    -   trained model
            Info        -   return code:
                            * -8, if both WStep=0 and MaxIts=0
                            * -2, if there is a point with class number
                                  outside of [0..NClasses-1].
                            * -1, if incorrect parameters was passed
                                  (NPoints<0, Restarts<1).
                            *  2, if task has been solved.
            Rep         -   training report.
            OOBErrors   -   out-of-bag generalization error estimate

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpebagginglbfgs(mlpensemble ensemble,
            double[,] xy,
            int npoints,
            double decay,
            int restarts,
            double wstep,
            int maxits,
            ref int info,
            mlptrain.mlpreport rep,
            mlptrain.mlpcvreport ooberrors)
        {
            info = 0;

            mlpebagginginternal(ensemble, xy, npoints, decay, restarts, wstep, maxits, false, ref info, rep, ooberrors);
        }


        /*************************************************************************
        Training neural networks ensemble using early stopping.

        INPUT PARAMETERS:
            Ensemble    -   model with initialized geometry
            XY          -   training set
            NPoints     -   training set size
            Decay       -   weight decay coefficient, >=0.001
            Restarts    -   restarts, >0.

        OUTPUT PARAMETERS:
            Ensemble    -   trained model
            Info        -   return code:
                            * -2, if there is a point with class number
                                  outside of [0..NClasses-1].
                            * -1, if incorrect parameters was passed
                                  (NPoints<0, Restarts<1).
                            *  6, if task has been solved.
            Rep         -   training report.
            OOBErrors   -   out-of-bag generalization error estimate

          -- ALGLIB --
             Copyright 10.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpetraines(mlpensemble ensemble,
            double[,] xy,
            int npoints,
            double decay,
            int restarts,
            ref int info,
            mlptrain.mlpreport rep)
        {
            int i = 0;
            int k = 0;
            int ccount = 0;
            int pcount = 0;
            double[,] trnxy = new double[0,0];
            double[,] valxy = new double[0,0];
            int trnsize = 0;
            int valsize = 0;
            mlpbase.multilayerperceptron network = new mlpbase.multilayerperceptron();
            int tmpinfo = 0;
            mlptrain.mlpreport tmprep = new mlptrain.mlpreport();
            int i_ = 0;
            int i1_ = 0;

            info = 0;

            if( (npoints<2 | restarts<1) | (double)(decay)<(double)(0) )
            {
                info = -1;
                return;
            }
            if( ensemble.issoftmax )
            {
                for(i=0; i<=npoints-1; i++)
                {
                    if( (int)Math.Round(xy[i,ensemble.nin])<0 | (int)Math.Round(xy[i,ensemble.nin])>=ensemble.nout )
                    {
                        info = -2;
                        return;
                    }
                }
            }
            info = 6;
            
            //
            // allocate
            //
            if( ensemble.issoftmax )
            {
                ccount = ensemble.nin+1;
                pcount = ensemble.nin;
            }
            else
            {
                ccount = ensemble.nin+ensemble.nout;
                pcount = ensemble.nin+ensemble.nout;
            }
            trnxy = new double[npoints-1+1, ccount-1+1];
            valxy = new double[npoints-1+1, ccount-1+1];
            mlpbase.mlpunserialize(ensemble.serializedmlp, network);
            rep.ngrad = 0;
            rep.nhess = 0;
            rep.ncholesky = 0;
            
            //
            // train networks
            //
            for(k=0; k<=ensemble.ensemblesize-1; k++)
            {
                
                //
                // Split set
                //
                do
                {
                    trnsize = 0;
                    valsize = 0;
                    for(i=0; i<=npoints-1; i++)
                    {
                        if( (double)(math.randomreal())<(double)(0.66) )
                        {
                            
                            //
                            // Assign sample to training set
                            //
                            for(i_=0; i_<=ccount-1;i_++)
                            {
                                trnxy[trnsize,i_] = xy[i,i_];
                            }
                            trnsize = trnsize+1;
                        }
                        else
                        {
                            
                            //
                            // Assign sample to validation set
                            //
                            for(i_=0; i_<=ccount-1;i_++)
                            {
                                valxy[valsize,i_] = xy[i,i_];
                            }
                            valsize = valsize+1;
                        }
                    }
                }
                while( !(trnsize!=0 & valsize!=0) );
                
                //
                // Train
                //
                mlptrain.mlptraines(network, trnxy, trnsize, valxy, valsize, decay, restarts, ref tmpinfo, tmprep);
                if( tmpinfo<0 )
                {
                    info = tmpinfo;
                    return;
                }
                
                //
                // save results
                //
                i1_ = (0) - (k*ensemble.wcount);
                for(i_=k*ensemble.wcount; i_<=(k+1)*ensemble.wcount-1;i_++)
                {
                    ensemble.weights[i_] = network.weights[i_+i1_];
                }
                i1_ = (0) - (k*pcount);
                for(i_=k*pcount; i_<=(k+1)*pcount-1;i_++)
                {
                    ensemble.columnmeans[i_] = network.columnmeans[i_+i1_];
                }
                i1_ = (0) - (k*pcount);
                for(i_=k*pcount; i_<=(k+1)*pcount-1;i_++)
                {
                    ensemble.columnsigmas[i_] = network.columnsigmas[i_+i1_];
                }
                rep.ngrad = rep.ngrad+tmprep.ngrad;
                rep.nhess = rep.nhess+tmprep.nhess;
                rep.ncholesky = rep.ncholesky+tmprep.ncholesky;
            }
        }


        /*************************************************************************
        Calculation of all types of errors

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        private static void mlpeallerrors(mlpensemble ensemble,
            double[,] xy,
            int npoints,
            ref double relcls,
            ref double avgce,
            ref double rms,
            ref double avg,
            ref double avgrel)
        {
            int i = 0;
            double[] buf = new double[0];
            double[] workx = new double[0];
            double[] y = new double[0];
            double[] dy = new double[0];
            int i_ = 0;
            int i1_ = 0;

            relcls = 0;
            avgce = 0;
            rms = 0;
            avg = 0;
            avgrel = 0;

            workx = new double[ensemble.nin-1+1];
            y = new double[ensemble.nout-1+1];
            if( ensemble.issoftmax )
            {
                dy = new double[0+1];
                bdss.dserrallocate(ensemble.nout, ref buf);
            }
            else
            {
                dy = new double[ensemble.nout-1+1];
                bdss.dserrallocate(-ensemble.nout, ref buf);
            }
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=ensemble.nin-1;i_++)
                {
                    workx[i_] = xy[i,i_];
                }
                mlpeprocess(ensemble, workx, ref y);
                if( ensemble.issoftmax )
                {
                    dy[0] = xy[i,ensemble.nin];
                }
                else
                {
                    i1_ = (ensemble.nin) - (0);
                    for(i_=0; i_<=ensemble.nout-1;i_++)
                    {
                        dy[i_] = xy[i,i_+i1_];
                    }
                }
                bdss.dserraccumulate(ref buf, y, dy);
            }
            bdss.dserrfinish(ref buf);
            relcls = buf[0];
            avgce = buf[1];
            rms = buf[2];
            avg = buf[3];
            avgrel = buf[4];
        }


        /*************************************************************************
        Internal bagging subroutine.

          -- ALGLIB --
             Copyright 19.02.2009 by Bochkanov Sergey
        *************************************************************************/
        private static void mlpebagginginternal(mlpensemble ensemble,
            double[,] xy,
            int npoints,
            double decay,
            int restarts,
            double wstep,
            int maxits,
            bool lmalgorithm,
            ref int info,
            mlptrain.mlpreport rep,
            mlptrain.mlpcvreport ooberrors)
        {
            double[,] xys = new double[0,0];
            bool[] s = new bool[0];
            double[,] oobbuf = new double[0,0];
            int[] oobcntbuf = new int[0];
            double[] x = new double[0];
            double[] y = new double[0];
            double[] dy = new double[0];
            double[] dsbuf = new double[0];
            int nin = 0;
            int nout = 0;
            int ccnt = 0;
            int pcnt = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;
            mlptrain.mlpreport tmprep = new mlptrain.mlpreport();
            mlpbase.multilayerperceptron network = new mlpbase.multilayerperceptron();
            int i_ = 0;
            int i1_ = 0;

            info = 0;

            
            //
            // Test for inputs
            //
            if( (!lmalgorithm & (double)(wstep)==(double)(0)) & maxits==0 )
            {
                info = -8;
                return;
            }
            if( ((npoints<=0 | restarts<1) | (double)(wstep)<(double)(0)) | maxits<0 )
            {
                info = -1;
                return;
            }
            if( ensemble.issoftmax )
            {
                for(i=0; i<=npoints-1; i++)
                {
                    if( (int)Math.Round(xy[i,ensemble.nin])<0 | (int)Math.Round(xy[i,ensemble.nin])>=ensemble.nout )
                    {
                        info = -2;
                        return;
                    }
                }
            }
            
            //
            // allocate temporaries
            //
            info = 2;
            rep.ngrad = 0;
            rep.nhess = 0;
            rep.ncholesky = 0;
            ooberrors.relclserror = 0;
            ooberrors.avgce = 0;
            ooberrors.rmserror = 0;
            ooberrors.avgerror = 0;
            ooberrors.avgrelerror = 0;
            nin = ensemble.nin;
            nout = ensemble.nout;
            if( ensemble.issoftmax )
            {
                ccnt = nin+1;
                pcnt = nin;
            }
            else
            {
                ccnt = nin+nout;
                pcnt = nin+nout;
            }
            xys = new double[npoints-1+1, ccnt-1+1];
            s = new bool[npoints-1+1];
            oobbuf = new double[npoints-1+1, nout-1+1];
            oobcntbuf = new int[npoints-1+1];
            x = new double[nin-1+1];
            y = new double[nout-1+1];
            if( ensemble.issoftmax )
            {
                dy = new double[0+1];
            }
            else
            {
                dy = new double[nout-1+1];
            }
            for(i=0; i<=npoints-1; i++)
            {
                for(j=0; j<=nout-1; j++)
                {
                    oobbuf[i,j] = 0;
                }
            }
            for(i=0; i<=npoints-1; i++)
            {
                oobcntbuf[i] = 0;
            }
            mlpbase.mlpunserialize(ensemble.serializedmlp, network);
            
            //
            // main bagging cycle
            //
            for(k=0; k<=ensemble.ensemblesize-1; k++)
            {
                
                //
                // prepare dataset
                //
                for(i=0; i<=npoints-1; i++)
                {
                    s[i] = false;
                }
                for(i=0; i<=npoints-1; i++)
                {
                    j = math.randominteger(npoints);
                    s[j] = true;
                    for(i_=0; i_<=ccnt-1;i_++)
                    {
                        xys[i,i_] = xy[j,i_];
                    }
                }
                
                //
                // train
                //
                if( lmalgorithm )
                {
                    mlptrain.mlptrainlm(network, xys, npoints, decay, restarts, ref info, tmprep);
                }
                else
                {
                    mlptrain.mlptrainlbfgs(network, xys, npoints, decay, restarts, wstep, maxits, ref info, tmprep);
                }
                if( info<0 )
                {
                    return;
                }
                
                //
                // save results
                //
                rep.ngrad = rep.ngrad+tmprep.ngrad;
                rep.nhess = rep.nhess+tmprep.nhess;
                rep.ncholesky = rep.ncholesky+tmprep.ncholesky;
                i1_ = (0) - (k*ensemble.wcount);
                for(i_=k*ensemble.wcount; i_<=(k+1)*ensemble.wcount-1;i_++)
                {
                    ensemble.weights[i_] = network.weights[i_+i1_];
                }
                i1_ = (0) - (k*pcnt);
                for(i_=k*pcnt; i_<=(k+1)*pcnt-1;i_++)
                {
                    ensemble.columnmeans[i_] = network.columnmeans[i_+i1_];
                }
                i1_ = (0) - (k*pcnt);
                for(i_=k*pcnt; i_<=(k+1)*pcnt-1;i_++)
                {
                    ensemble.columnsigmas[i_] = network.columnsigmas[i_+i1_];
                }
                
                //
                // OOB estimates
                //
                for(i=0; i<=npoints-1; i++)
                {
                    if( !s[i] )
                    {
                        for(i_=0; i_<=nin-1;i_++)
                        {
                            x[i_] = xy[i,i_];
                        }
                        mlpbase.mlpprocess(network, x, ref y);
                        for(i_=0; i_<=nout-1;i_++)
                        {
                            oobbuf[i,i_] = oobbuf[i,i_] + y[i_];
                        }
                        oobcntbuf[i] = oobcntbuf[i]+1;
                    }
                }
            }
            
            //
            // OOB estimates
            //
            if( ensemble.issoftmax )
            {
                bdss.dserrallocate(nout, ref dsbuf);
            }
            else
            {
                bdss.dserrallocate(-nout, ref dsbuf);
            }
            for(i=0; i<=npoints-1; i++)
            {
                if( oobcntbuf[i]!=0 )
                {
                    v = (double)1/(double)oobcntbuf[i];
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        y[i_] = v*oobbuf[i,i_];
                    }
                    if( ensemble.issoftmax )
                    {
                        dy[0] = xy[i,nin];
                    }
                    else
                    {
                        i1_ = (nin) - (0);
                        for(i_=0; i_<=nout-1;i_++)
                        {
                            dy[i_] = v*xy[i,i_+i1_];
                        }
                    }
                    bdss.dserraccumulate(ref dsbuf, y, dy);
                }
            }
            bdss.dserrfinish(ref dsbuf);
            ooberrors.relclserror = dsbuf[0];
            ooberrors.avgce = dsbuf[1];
            ooberrors.rmserror = dsbuf[2];
            ooberrors.avgerror = dsbuf[3];
            ooberrors.avgrelerror = dsbuf[4];
        }


    }
    public class pca
    {
        /*************************************************************************
        Principal components analysis

        Subroutine  builds  orthogonal  basis  where  first  axis  corresponds  to
        direction with maximum variance, second axis maximizes variance in subspace
        orthogonal to first axis and so on.

        It should be noted that, unlike LDA, PCA does not use class labels.

        INPUT PARAMETERS:
            X           -   dataset, array[0..NPoints-1,0..NVars-1].
                            matrix contains ONLY INDEPENDENT VARIABLES.
            NPoints     -   dataset size, NPoints>=0
            NVars       -   number of independent variables, NVars>=1

        �������� ���������:
            Info        -   return code:
                            * -4, if SVD subroutine haven't converged
                            * -1, if wrong parameters has been passed (NPoints<0,
                                  NVars<1)
                            *  1, if task is solved
            S2          -   array[0..NVars-1]. variance values corresponding
                            to basis vectors.
            V           -   array[0..NVars-1,0..NVars-1]
                            matrix, whose columns store basis vectors.

          -- ALGLIB --
             Copyright 25.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void pcabuildbasis(double[,] x,
            int npoints,
            int nvars,
            ref int info,
            ref double[] s2,
            ref double[,] v)
        {
            double[,] a = new double[0,0];
            double[,] u = new double[0,0];
            double[,] vt = new double[0,0];
            double[] m = new double[0];
            double[] t = new double[0];
            int i = 0;
            int j = 0;
            double mean = 0;
            double variance = 0;
            double skewness = 0;
            double kurtosis = 0;
            int i_ = 0;

            info = 0;
            s2 = new double[0];
            v = new double[0,0];

            
            //
            // Check input data
            //
            if( npoints<0 | nvars<1 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // Special case: NPoints=0
            //
            if( npoints==0 )
            {
                s2 = new double[nvars-1+1];
                v = new double[nvars-1+1, nvars-1+1];
                for(i=0; i<=nvars-1; i++)
                {
                    s2[i] = 0;
                }
                for(i=0; i<=nvars-1; i++)
                {
                    for(j=0; j<=nvars-1; j++)
                    {
                        if( i==j )
                        {
                            v[i,j] = 1;
                        }
                        else
                        {
                            v[i,j] = 0;
                        }
                    }
                }
                return;
            }
            
            //
            // Calculate means
            //
            m = new double[nvars-1+1];
            t = new double[npoints-1+1];
            for(j=0; j<=nvars-1; j++)
            {
                for(i_=0; i_<=npoints-1;i_++)
                {
                    t[i_] = x[i_,j];
                }
                basestat.samplemoments(t, npoints, ref mean, ref variance, ref skewness, ref kurtosis);
                m[j] = mean;
            }
            
            //
            // Center, apply SVD, prepare output
            //
            a = new double[Math.Max(npoints, nvars)-1+1, nvars-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    a[i,i_] = x[i,i_];
                }
                for(i_=0; i_<=nvars-1;i_++)
                {
                    a[i,i_] = a[i,i_] - m[i_];
                }
            }
            for(i=npoints; i<=nvars-1; i++)
            {
                for(j=0; j<=nvars-1; j++)
                {
                    a[i,j] = 0;
                }
            }
            if( !svd.rmatrixsvd(a, Math.Max(npoints, nvars), nvars, 0, 1, 2, ref s2, ref u, ref vt) )
            {
                info = -4;
                return;
            }
            if( npoints!=1 )
            {
                for(i=0; i<=nvars-1; i++)
                {
                    s2[i] = math.sqr(s2[i])/(npoints-1);
                }
            }
            v = new double[nvars-1+1, nvars-1+1];
            blas.copyandtranspose(vt, 0, nvars-1, 0, nvars-1, ref v, 0, nvars-1, 0, nvars-1);
        }


    }
}

