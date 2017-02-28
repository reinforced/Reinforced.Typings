Prism.languages.cshtml = Prism.languages.extend('markup', {
	'razor_comment': {
        pattern: /@\*[\w\W]*?\*@/,
        greedy:false,
        inside: {
            razor_comment_begin: /@\*/,
            razor_comment_end: /\*@/
        }
    },
    'razor_block': {
      pattern: /@\{[\w\W]*?\}(?=[\s\S]*(@|\<))/,
      greedy:true,
      inside :{      
          'razor_borders': /@\{/,
          'razor_end': {
              pattern: /\}$/ ,
              alias:'razor_borders'
          }          
      }
    },
    
    'razor_keyword': /@(model|using|section|if|foreach)/,    
    'razor_inline': {        
        greedy:false,
        pattern: /@[\w]([\w\.]|\([\w\"\~\\\/\s\,\:\(\$\.\)]*\))+/,
        inside: {
            'razor_borders': /^@/,            
        }
    }
});
Prism.languages.cshtml['razor_block'].inside.rest = Prism.util.clone(Prism.languages.csharp);
Prism.languages.cshtml['razor_inline'].inside.rest = Prism.util.clone(Prism.languages.csharp);
Prism.languages.cshtml['razor_inline'].inside['razor_pageclass'] = /\b(Html|Url|Ajax|this|Model|ViewBag|Styles|Scripts)\b/;
Prism.languages.cshtml['razor_block'].inside['razor_pageclass'] = /\b(Html|Url|Ajax|this|Model|ViewBag|Styles|Scripts)\b/;