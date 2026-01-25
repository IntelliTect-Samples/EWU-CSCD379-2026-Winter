import { ref } from 'vue'

export const WORD_LIST = [
  'about','above','abuse','actor','acute','admin','admit','adopt','adult','after','again',
  'agent','agree','ahead','alarm','album','alert','alike','alive','allow','alone',
  'along','alter','among','anger','angle','angry','apart','apple','apply','arena',
  'argue','arise','array','aside','asset','audio','avoid',

  'baker','basic','beach','began','begin','being','below','bench','billy','birth',
  'black','blame','blind','block','blood','board','brain','brand','bread','break',
  'brick','brief','bring','broad','brown','build','buyer',

  'cable','carry','catch','cause','chain','chair','chart','chase','cheap','check',
  'chest','chief','child','china','claim','class','clean','clear','click','clock',
  'close','cloud','coach','coast','could','count','court','cover','crack','craft',
  'crash','cream','crime','cross','crowd','crown','curve',

  'dance','death','delay','depth','dirty','doubt','draft','drama','dream','dress',
  'drink','drive','drove',

  'eager','early','earth','eight','elite','empty','enemy','enjoy','enter','equal',
  'error','event','every','exact','exist','extra',

  'faith','false','fault','fiber','field','fifth','fifty','fight','final','first',
  'flame','flash','fleet','floor','focus','force','forth','frame','fresh','front',
  'fruit','fully','funny',

  'giant','given','glass','globe','going','grace','grade','grain','grand','grant',
  'grape','grass','great','green','gross','group','guard','guess',

  'happy','heart','heavy','honey','horse','hotel','house','human','humor',

  'ideal','image','index','inner','input','issue',

  'joint','judge','juice',

  'knife','knock','known',

  'label','labor','large','laser','later','laugh','layer','learn','least','leave',
  'legal','lemon','level','light','limit','local','logic','loose','lunch',

  'magic','major','maker','march','match','maybe','metal','might','minor','model',
  'money','month','moral','motor','mouse','mouth','movie','mummy','music',

  'naked','nerve','never','night','noise','north','novel','nurse',

  'ocean','offer','often','order','other','ought','outer','owner',

  'panel','paper','party','peace','piano','phase','phone','photo','piece','pilot',
  'pitch','place','plain','plane','plant','plate','point','power','press','price',
  'pride','prime','print','prior','prize','proof','proud',

  'queen','quick','quiet','quite',

  'radio','raise','range','rapid','ratio','reach','react','ready','refer','right',
  'river','round','route','royal','rugby',

  'scale','scene','scope','score','sense','serve','seven','shall','shape','share',
  'sharp','sheet','shelf','shell','shift','shiny','shirt','shock','shoot','short','shown',
  'sight','since','skill','sleep','slide','small','smart','smile','smoke','solid',
  'solve','sound','south','space','spare','speak','speed','spend','spice','split',
  'sport','staff','stage','stand','start','state','steam','steel','stein','stick','still','stock','stone',
  'store','storm','strip','study','stuff','style','sugar','suite','super','sweet',

  'table','taint','taste','teach','teeth','thank','their','theme','there','thick','thing',
  'think','third','those','three','throw','tight','timer','tired','title','today',
  'topic','total','touch','tower','track','trade','train','treat','trend','trial',
  'trust','truth',

  'under','union','unity','until','upper','urban','usage','usual',

  'value','video','visit','vital','voice',

  'waste','watch','water','wheel','where','which','while','white','whole','whose',
  'woman','women','world','worry','worth','would','write','wrong',

  'yield','young',

  'zebra'
]

// <><><><> Dictionary of valid guesses pulled from a text file
export const VALID_GUESSES = ref<string[]>([]);

// <><><><> Load the dictionary
export const loadDictionary = async () => {
  if (import.meta.server) return;

  try {
    // <><><><> Fetch the file <><><><>
    const response = await fetch('/valid-words.txt'); 
    
    if (!response.ok) {
      throw new Error('Could not find valid-words.txt');
    } 
    
    const rawText = await response.text();
    
    // <><><><> Convert file into array <><><><>
    const dictionary = rawText
      .split(/\r?\n/)
      .map(word => word.trim().toLowerCase())
      .filter(word => word.length === 5);

    // <><><><> Populate valid guesses with both the word list and the valid guesses <><><><>
    VALID_GUESSES.value = [
      ...WORD_LIST.map(w => w.toLowerCase()),
      ...dictionary
    ];
    
    console.log(`Dictionary Ready: ${VALID_GUESSES.value.length} words stored in VALID_GUESSES.`);
  } catch (error) {
    console.error("Dictionary failed to load:", error);
    VALID_GUESSES.value = WORD_LIST.map(w => w.toLowerCase());
  }
}

export const generateHint = async (word: string): Promise<string> => {
  try {
    const response = await fetch(`https://api.dictionaryapi.dev/api/v2/entries/en/${word}`)
    if (!response.ok) {
      throw new Error('Definition could not be found');
    }
    const data = await response.json();
    let definition = data[0].meanings[0].definitions[0].definition
    const safeHint = definition.replace(new RegExp(word, 'gi'), '____');
    return safeHint.length > 150 ? safeHint.substring(0, 150) + '...' : safeHint;
  } catch (error){
    console.error("Hint Error: ", error);
    return "No definition was found for this word"
  }
};

export interface WordData {
  word: string;
  hint: string;
}

export const getRandomWord = () => {
  const index = Math.floor(Math.random() * WORD_LIST.length);
  const selectedWord = WORD_LIST[index] || 'APPLE';
  return { 
    word: selectedWord.toUpperCase(), 
    hint: '' 
  };
}